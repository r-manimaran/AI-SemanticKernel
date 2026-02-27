
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;

var builder = Host.CreateApplicationBuilder();
builder.Services
    .AddChatClient(new OllamaApiClient(
        new Uri("http://localhost:11434"),"llama3.2-vision:latest"));
var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();
//var message = new ChatMessage(ChatRole.User, "Explain the use of Dapper in .NET?");
//var response = await chatClient.GetResponseAsync([message]);

//var message = new ChatMessage(ChatRole.User, "What's in this Image?");
var message = new ChatMessage(ChatRole.User,
    """
        Analyze the attached receipt image and extract the following details in a structured JSON format:

    GasStationName
    Date and Time 
    Gallons Filled
    Price per Gallon
    Total Amount Spent
    PaymentMode

    Follow these rules strictly:

    For Gallons Filled, only extract the numeric value that appears directly below to the label "Gallons".

    Ignore numbers that appear under "Pump" or other headings.

    Do NOT use column headers (e.g., "Pump", "Gallons", "Price") as values.

    Use spatial alignment and proximity to determine correct fields.
    Have the DateandTime field formatted as mm/dd/yy HH:MM

    If any field is missing, return it as null. Ensure values are accurate and formatted clearly.

    """);

message.Contents.Add(new DataContent(File.ReadAllBytes("receipts/costco2.jpg"),"image/jpg"));
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
try
{
    var response = await chatClient.GetResponseAsync<Receipt>(new[] { message },null,true, cts.Token);
    Console.WriteLine(response.Text);
}
catch (TaskCanceledException ex) when (!cts.IsCancellationRequested)
{
    // This indicates the underlying HttpClient timed out (or was cancelled by the library)
    Console.WriteLine("Request cancelled by HttpClient (likely timeout). Increase HttpClient.Timeout or check the server.");
    Console.WriteLine(ex);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation cancelled by local cancellation token.");
}

public class Receipt
{
    public string GasStationName {  get; set; }
    public string DateTime { get; set; }
    public decimal GallonsFilled { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMode ModeOfPayment { get; set; }
}

public enum PaymentMode
{
    Cash,
    CreditCard
}