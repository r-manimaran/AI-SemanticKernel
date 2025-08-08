var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FoundryAIAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var chat = builder.AddAzureAIFoundry("Foundry-AI-Local")
                  .RunAsFoundryLocal()
                  .AddDeployment("chat", "qwen2.5-1.5b", "1", "Microsoft");

builder.AddProject<Projects.FoundryAIAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithReference(chat)
    .WaitFor(apiService)
    .WaitFor(chat);


builder.Build().Run();
