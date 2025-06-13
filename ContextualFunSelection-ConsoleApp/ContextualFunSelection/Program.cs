using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Functions;
using OllamaSharp;

var ollamaUrl = "http://localhost:11434/";
string modelId = "llama3.2:3b";

IEmbeddingGenerator<string,Embedding<float>> eg = new OllamaApiClient(ollamaUrl, modelId);

Uri endpointOllama = new (ollamaUrl);

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.Services.AddOllamaChatCompletion(
   modelId,
    endpointOllama
);

var kernel = kernelBuilder.Build();

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddConsole();
});

ChatCompletionAgent claimAgent = new()
{
    Name = "InsuranceClaimAgent",
    Instructions = @"You work in a large insurance company. You analysis claims data made by customers to understand the current trends",
    Kernel = kernel,
    Arguments = new(new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions
        {
            RetainArgumentTypes = true
        })
    })
};


ChatHistoryAgentThread chaThread = new();
chaThread.AIContextProviders.Add(new ContextualFunctionProvider(
    vectorStore: new InMemoryVectorStore(new InMemoryVectorStoreOptions()
    {
        EmbeddingGenerator = eg
    }),
    vectorDimensions: 4096,
    functions: AIFunctions(),
    maxNumberOfFunctions: 3,
    loggerFactory:loggerFactory
    ));

ChatMessageContent msg = await claimAgent.InvokeAsync("Identify trends in insurance claims for this year", chaThread).FirstAsync();
Console.WriteLine(msg.Content);

ChatMessageContent msg2 = await claimAgent.InvokeAsync("Find the address of 345 Buckland Hills Drive, Manchester, CT - 06042 ?", chaThread).FirstAsync();
Console.WriteLine(msg2.Content);
static IReadOnlyList<AIFunction> AIFunctions()
{
    List<AIFunction> claimsFunctions = [
        AIFunctionFactory.Create(()=> """
        [
        {
          "claimType":"Painting insurance",
          "claimValue": 100,000,
          "country":"Spain",
          "claimAccepted": true
        },
        {
          "claimType":"Car insurance",
          "claimValue": 30,000,
          "country":"Austria",
          "claimAccepted": false
        },
        {
          "claimType":"Device insurance",
          "claimValue": 500,
          "country":"France",
          "claimAccepted": true
        },
        {
          "claimType":"Device insurance",
          "claimValue": 900,
          "country":"UK",
          "claimAccepted": true
        }
        ]
        ""","GetClaimsData")];
    List<AIFunction> msFunctions = [
        AIFunctionFactory.Create((DateTime eventDateTime, string text) => "created Microsoft outlook appointment", "CreateOutlookAppointment"),
        AIFunctionFactory.Create((string text) => "Message in Microsoft Teams sent","SendMessageInTeams")
        ];
    // Placeholder for Google functions, if any
    List<AIFunction> googleFunctions = [
        AIFunctionFactory.Create((string fileName) => "Added file to the google drive", "AddToGoogleDrive"),
        AIFunctionFactory.Create((string text) => "Email sent using gmail", "SendEmailWithGmail"),
        AIFunctionFactory.Create((string postcode) => "Address Test street one returned", "FindAddressUsingGoogleMaps")
        ];

    // Placeholder for insurance claims trend functions, if any
    List<AIFunction> insuranceClaimsTrendFunctions = [
        AIFunctionFactory.Create((string text)=>"Claims are mostly made in Europe and are usually accepted","CollectClaimTrends" ),
        AIFunctionFactory.Create((string text)=>"Average value of claim is above 50,000 euros","ClaimValueTrends" ),
        AIFunctionFactory.Create((string text)=>"Claims this year vary from car to laptop insurance","ClaimTypeTrends" )
    ];
    return [.. claimsFunctions, .. msFunctions, .. googleFunctions, .. insuranceClaimsTrendFunctions];
}