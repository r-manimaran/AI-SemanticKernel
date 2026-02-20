using Microsoft.SemanticKernel;
using ProjectDescGenerator;
using System.Text.Json;

// 1. Initialize the kernel with OpenAI chat completion service
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(AppConfig.ModelName, AppConfig.ApiKey);

var kernel = builder.Build();

// 2. Define a prompt template
string promptTemplate = @"
Analyze the following .NET project and provide a concise description of its purpose, key features, and technologies used.
    [PROJECT FILE CONTENT]
    {{$fileContent}}

### OUTPUT INSTRUCTIONS:
    - Return ONLY a valid JSON object.
    - DO NOT include markdown code blocks (```json).
    - DO NOT include any leading or trailing characters like braces or backticks.
    - The output must be directly parsable by JsonSerializer.Deserialize.

JSON Structure:
    {
      ""Title"": ""string"",
      ""Description"": ""string"",
      ""TechnologyUsed"": [""C#"", "".NET 8"", ""Entity Framework"", ""etc""]
    }
";
var generateDescription = kernel.CreateFunctionFromPrompt(promptTemplate);

// 3. Scan Folders and read .csproj file content
string rootPath = AppConfig.RootPath;
var results = new List<ProjectMetadata>();

foreach (var dir in Directory.GetDirectories(rootPath))
{
    var csprojFiles = Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories);
    foreach (var csprojFile in csprojFiles)
    {
        string fileContent = await File.ReadAllTextAsync(csprojFile);

        // 4. Invoke the function with the .csproj file content
        var result = await kernel.InvokeAsync(generateDescription, new() { ["fileContent"] = fileContent });

        // 5. Deserialize the JSON response into ProjectMetadata object
        var projectMetadata = System.Text.Json.JsonSerializer.Deserialize<ProjectMetadata>(result.ToString());
        if (projectMetadata != null)
        {
            results.Add(projectMetadata);
        }
    }
}

Console.WriteLine(JsonSerializer.Serialize(results,new JsonSerializerOptions {WriteIndented = true }));

