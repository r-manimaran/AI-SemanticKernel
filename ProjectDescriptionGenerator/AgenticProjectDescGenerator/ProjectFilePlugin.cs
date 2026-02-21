using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AgenticProjectDescGenerator;

public class ProjectFilePlugin
{
    [KernelFunction, Description("Gets the list of project folders in the specified root path.")]
    public List<string> GetProjectFolders(string rootPath)
        => Directory.GetDirectories(rootPath).ToList();

    [KernelFunction, Description("Reads the content of the .csproj file in the specified folder path.")]
    public async Task<string> ReadProjectFile(string folderPath)
    {
        var file = Directory.GetFiles(folderPath,"*.csproj", SearchOption.AllDirectories).FirstOrDefault();

        return file != null ? await File.ReadAllTextAsync(file) : "No .csproj found.";
    }

    [KernelFunction, Description("Reads the content of the Readme.md file in the specified folder path.")]
    public async Task<string> ReadReadMeFile(string folderPath)
    {
        // 1. Check root directory first
        var rootFile = Directory
            .GetFiles(folderPath, "*.md", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (rootFile != null)
        {
            return await File.ReadAllTextAsync(rootFile);
        }

        // 2. Then check subdirectories
        var subFile = Directory
            .GetFiles(folderPath, "*.md", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (subFile != null)
        {
            return await File.ReadAllTextAsync(subFile);
        }

        return "No .md found.";
    }
}
