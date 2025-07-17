using Microsoft.AI.Foundry.Local;
using OpenAI;
using System.ClientModel;

var alias = "qwen2.5-1.5b";

var manager = await FoundryLocalManager.StartModelAsync(aliasOrModelId: alias);

var model = await manager.GetModelInfoAsync(aliasOrModelId: alias);
ApiKeyCredential key = new ApiKeyCredential(manager.ApiKey);
OpenAIClient client = new OpenAIClient(key, new OpenAIClientOptions
{
    Endpoint = manager.Endpoint
});
var chatClient = client.GetChatClient(model?.ModelId);

var completionUpdates = chatClient.CompleteChatStreaming("Why is the sky blue?");
Console.Write($"[Assistant]");
foreach(var complettionUpdate in completionUpdates)
{
    if(complettionUpdate.ContentUpdate.Count > 0)
    {
        Console.Write(complettionUpdate.ContentUpdate[0].Text);
    }
}