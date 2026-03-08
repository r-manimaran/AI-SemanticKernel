using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text;

namespace McpServer.Services;

public class DockerService
{
    private readonly DockerClient _client;

    public DockerService()
    {
        // Connects to Local docker engine automatically
        // Windows : npipe://./pipe/docker_engine
        _client = new DockerClientConfiguration().CreateClient();
    }

    public async Task<IList<ContainerListResponse>> GetContainersAsync(bool all = true)
    {
        return await _client.Containers.ListContainersAsync(new ContainersListParameters { All = all });
    }

    public async Task<ContainerInspectResponse> InspectContainerAsync(string containerId)
    {
        return await _client.Containers.InspectContainerAsync(containerId);
    }

    // ── Logs ──────────────────────────────────────────────────

    //public async Task<string> GetLogsAsync(string containerId, int tail = 50)
    //{
    //    var parameters = new ContainerLogsParameters
    //    {
    //        ShowStdout = true,
    //        ShowStderr = true,
    //        Tail = tail.ToString(),
    //        Timestamps = true
    //    };

    //    var stream = await _client.Containers.GetContainerLogsAsync(containerId, parameters, CancellationToken.None);

    //    var (stdout, stderr) = await stream.ReadOutputToEndAsync(CancellationToken.None);

    //    var result = new System.Text.StringBuilder();
    //    if (!string.IsNullOrWhiteSpace(stdout)) result.AppendLine(stdout);
    //    if (!string.IsNullOrWhiteSpace(stderr)) result.AppendLine($"[STDERR]\n{stderr}");

    //    return result.ToString();
    //}

    // ✅ NEW - fixed
    public async Task<string> GetLogsAsync(string containerId, int tail = 50)
    {
        var parameters = new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Tail = tail.ToString(),
            Timestamps = true
        };

        // GetContainerLogsAsync now returns a raw MultiplexedStream
        var multiplexedStream = await _client.Containers.GetContainerLogsAsync(
            containerId,
            tty: false,                         // ← required parameter in newer versions
            parameters,
            CancellationToken.None);

        var sb = new StringBuilder();

        // Read line by line using ReadOutputAsync
        var buffer = new byte[4096];
        while (true)
        {
            var result = await multiplexedStream.ReadOutputAsync(
                buffer, 0, buffer.Length, CancellationToken.None);

            if (result.EOF)
                break;

            var line = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (result.Target == MultiplexedStream.TargetStream.StandardError)
                sb.AppendLine($"[STDERR] {line}");
            else
                sb.AppendLine(line);
        }

        return sb.ToString();
    }


    // ── Stats ─────────────────────────────────────────────────

    public async Task<ContainerStatsResponse> GetStatsAsync(string containerId)
    {
        ContainerStatsResponse? stats = null;

        await _client.Containers.GetContainerStatsAsync(
            containerId,
            new ContainerStatsParameters { Stream = false },
            new Progress<ContainerStatsResponse>(s => stats = s),
            CancellationToken.None);

        return stats!;
    }
}
