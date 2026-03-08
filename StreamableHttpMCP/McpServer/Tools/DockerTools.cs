using Docker.DotNet.Models;
using McpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

namespace McpServer.Tools;

[McpServerToolType]
public class DockerTools(DockerService docker)
{
    // ── 1. List Containers ────────────────────────────────────

    [McpServerTool, Description(
        "List all Docker containers on the local machine with their status, " +
        "image, ports, and uptime. Use this for any question about running containers.")]
    public async Task<string> ListContainers(
        [Description("true = show all containers including stopped ones, false = only running")]
        bool all = true)
    {
        var containers = await docker.GetContainersAsync(all);

        if (!containers.Any())
            return "No containers found.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {containers.Count} container(s):\n");

        foreach (var c in containers)
        {
            var name = c.Names.FirstOrDefault()?.TrimStart('/') ?? "unknown";
            var ports = c.Ports.Any()
                ? string.Join(", ", c.Ports.Select(p => $"{p.PublicPort}→{p.PrivatePort}/{p.Type}"))
                : "none";
            var uptime = c.Status;
           // var created = DateTimeOffset.FromUnixTimeSeconds(c.Created).ToString("yyyy-MM-dd HH:mm");

            var created = c.Created.ToString("yyyy-MM-dd HH:mm");

            sb.AppendLine($"🐳 Name    : {name}");
            sb.AppendLine($"   ID      : {c.ID[..12]}");
            sb.AppendLine($"   Image   : {c.Image}");
            sb.AppendLine($"   Status  : {c.State.ToUpper()} — {uptime}");
            sb.AppendLine($"   Ports   : {ports}");
            sb.AppendLine($"   Created : {created}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // ── 2. Get Container Details ──────────────────────────────

    [McpServerTool, Description(
        "Get detailed information about a specific Docker container including " +
        "environment variables, mounts, network settings, and restart policy.")]
    public async Task<string> GetContainerDetails(
        [Description("Container name or ID")] string containerNameOrId)
    {
        var containers = await docker.GetContainersAsync(all: true);
        var match = FindContainer(containers, containerNameOrId);

        if (match is null)
            return $"Container '{containerNameOrId}' not found.";

        var details = await docker.InspectContainerAsync(match.ID);

        var sb = new StringBuilder();
        sb.AppendLine($"📦 Container: {containerNameOrId}");
        sb.AppendLine($"   Full ID    : {details.ID[..12]}");
        sb.AppendLine($"   Image      : {details.Config.Image}");
        sb.AppendLine($"   Status     : {details.State.Status.ToUpper()}");
        sb.AppendLine($"   Started    : {details.State.StartedAt}");
        sb.AppendLine($"   Restart    : {details.HostConfig.RestartPolicy.Name}");
        sb.AppendLine($"   Platform   : {details.Platform}");

        if (details.Config.Env?.Any() == true)
        {
            sb.AppendLine("\n   Environment Variables:");
            foreach (var env in details.Config.Env.Take(10)) // limit sensitive data
                sb.AppendLine($"     {env}");
        }

        if (details.Mounts?.Any() == true)
        {
            sb.AppendLine("\n   Mounts:");
            foreach (var m in details.Mounts)
                sb.AppendLine($"     {m.Source} → {m.Destination} ({m.Mode})");
        }

        return sb.ToString();
    }

    // ── 3. Get Container Logs ─────────────────────────────────

    [McpServerTool, Description(
        "Fetch recent logs from a specific Docker container. " +
        "Use this to debug errors, check startup output, or monitor activity.")]
    public async Task<string> GetContainerLogs(
        [Description("Container name or ID")] string containerNameOrId,
        [Description("Number of recent log lines to return (default 50, max 500)")]
        int lines = 50)
    {
        var containers = await docker.GetContainersAsync(all: true);
        var match = FindContainer(containers, containerNameOrId);

        if (match is null)
            return $"Container '{containerNameOrId}' not found.";

        lines = Math.Min(lines, 500); // safety cap
        var logs = await docker.GetLogsAsync(match.ID, lines);

        return string.IsNullOrWhiteSpace(logs)
            ? $"No logs found for '{containerNameOrId}'."
            : $"📋 Last {lines} lines from '{containerNameOrId}':\n\n{logs}";
    }

    // ── 4. Get Container Stats (CPU / Memory) ─────────────────

    [McpServerTool, Description(
        "Get real-time CPU and memory usage stats for a running Docker container.")]
    public async Task<string> GetContainerStats(
        [Description("Container name or ID")] string containerNameOrId)
    {
        var containers = await docker.GetContainersAsync(all: false); // running only
        var match = FindContainer(containers, containerNameOrId);

        if (match is null)
            return $"Container '{containerNameOrId}' is not running or not found.";

        var stats = await docker.GetStatsAsync(match.ID);

        // Calculate CPU %
        var cpuDelta = stats.CPUStats.CPUUsage.TotalUsage - stats.PreCPUStats.CPUUsage.TotalUsage;
        var systemDelta = stats.CPUStats.SystemUsage - stats.PreCPUStats.SystemUsage;
        var cpuCount = (double)(stats.CPUStats.OnlineCPUs > 0 ? stats.CPUStats.OnlineCPUs : 1);
        var cpuPercent = systemDelta > 0 ? (cpuDelta / (double)systemDelta) * cpuCount * 100.0 : 0;

        // Memory
        var memUsageMb = stats.MemoryStats.Usage / 1024.0 / 1024.0;
        var memLimitMb = stats.MemoryStats.Limit / 1024.0 / 1024.0;
        var memPercent = memLimitMb > 0 ? (memUsageMb / memLimitMb) * 100 : 0;

        return $"""
            📊 Stats for '{containerNameOrId}':

               CPU Usage  : {cpuPercent:F2}%
               Memory     : {memUsageMb:F1} MB / {memLimitMb:F1} MB ({memPercent:F1}%)
               PIDs       : {stats.PidsStats.Current}
            """;
    }

    // ── 5. Get Docker Summary ─────────────────────────────────

    [McpServerTool, Description(
        "Get a high-level health summary of all Docker containers — " +
        "how many are running, stopped, or unhealthy.")]
    public async Task<string> GetDockerSummary()
    {
        var all = await docker.GetContainersAsync(all: true);
        var running = all.Count(c => c.State == "running");
        var stopped = all.Count(c => c.State == "exited");
        var paused = all.Count(c => c.State == "paused");
        var other = all.Count - running - stopped - paused;

        var sb = new StringBuilder();
        sb.AppendLine("🐳 Docker Summary");
        sb.AppendLine($"   Total      : {all.Count}");
        sb.AppendLine($"   ✅ Running  : {running}");
        sb.AppendLine($"   🛑 Stopped  : {stopped}");
        sb.AppendLine($"   ⏸ Paused   : {paused}");
        if (other > 0)
            sb.AppendLine($"   ⚠️  Other   : {other}");

        if (running > 0)
        {
            sb.AppendLine("\n   Running containers:");
            foreach (var c in all.Where(c => c.State == "running"))
            {
                var name = c.Names.FirstOrDefault()?.TrimStart('/') ?? c.ID[..12];
                sb.AppendLine($"     • {name} ({c.Image}) — {c.Status}");
            }
        }

        return sb.ToString();
    }

    // ── Helper ────────────────────────────────────────────────

    private static ContainerListResponse? FindContainer(
        IList<ContainerListResponse> containers, string nameOrId)
    {
        return containers.FirstOrDefault(c =>
            c.ID.StartsWith(nameOrId, StringComparison.OrdinalIgnoreCase) ||
            c.Names.Any(n => n.TrimStart('/').Equals(nameOrId, StringComparison.OrdinalIgnoreCase)));
    }
}
