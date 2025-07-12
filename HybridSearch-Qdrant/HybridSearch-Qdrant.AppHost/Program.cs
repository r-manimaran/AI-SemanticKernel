var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama", 11434)
                    .WithLifetime(ContainerLifetime.Persistent).AddModel("llava");

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.BlazorHybridSearchApp>("blazorhybridsearchapp")
       .WithReference(qdrant)
       .WaitFor(qdrant)
       .WithReference(ollama)
       .WaitFor(ollama);

builder.Build().Run();
