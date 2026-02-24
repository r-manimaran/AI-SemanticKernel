using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Text;

namespace AgenticProjectDescGenerator;

public class ProjectContextPlugin
{
    [KernelFunction, Description("Gathers multiple context signals from a project folder.")]
    public async Task<string> GetProjectContext(string folderPath)
    {
        var sb = new StringBuilder();

        // Signal 1: Directory Structure (limit depth to 2)
        var directories = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                                   .Select(Path.GetFileName);

        sb.AppendLine($"[Directory Structure]: {string.Join(", ", directories)}");

        // Signal 2: README content (First 500 characters)
        var readme = Directory.GetFiles(folderPath, "README.md", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (readme != null)
            sb.AppendLine($"[README SNIPPET]: {new string((await File.ReadAllTextAsync(readme)).Take(500).ToArray())}");

        // Signal 3: Project File (Metadata)
        var csproj = Directory.GetFiles(folderPath, "*.csproj").FirstOrDefault();
        if (csproj != null)
            sb.AppendLine($"[PROJECT FILE]: {await File.ReadAllTextAsync(csproj)}");

        // Signal 4: Program.cs (Architecture)
        var program = Directory.GetFiles(folderPath, "Program.cs", SearchOption.AllDirectories).FirstOrDefault();
        if (program != null)
            sb.AppendLine($"[ENTRY POINT SNIPPET]: {new string((await File.ReadAllTextAsync(program)).Take(500).ToArray())}");

        return sb.ToString();
    }
}
