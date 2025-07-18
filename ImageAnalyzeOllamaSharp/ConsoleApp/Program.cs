using Microsoft.Extensions.AI;
using OllamaSharp;

string ollamaUrl = "http://localhost:11434/";
string modelId = "gemma3:12b";

IChatClient chatClient = new OllamaApiClient(ollamaUrl, modelId);

var prompt = "How many cars are in the picture?";

DataContent imgDC = new (File.ReadAllBytes("cars.jpg"), "image/jpeg");

List<ChatMessage> messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.User, prompt),
    new ChatMessage(ChatRole.User, [imgDC])
};

ChatResponse? response = await chatClient.GetResponseAsync(messages);

Console.WriteLine($"Result: {response?.Text}");  