var builder = DistributedApplication.CreateBuilder(args);

// var llm = builder.AddOllama("ollama").AddHuggingFaceModel("phi-3-mini", "ollama/phi-3-mini");

var qdrant = builder.AddQdrant("qdrant");

builder.AddProject<Projects.WebApi>("webapi").WithReference(qdrant).WaitFor(qdrant);

builder.Build().Run();
