# Image Analysis with Ollama and .NET

This project demonstrates how to use Ollama with .NET to analyze images using multimodal AI models. The application sends an image to a locally running Ollama model and asks questions about the image content.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Ollama](https://ollama.ai/) installed and running locally
- A multimodal model like `gemma3:12b` pulled in Ollama

## Setup

1. Install Ollama from [ollama.ai](https://ollama.ai/)
2. Pull the required model:
   ```
   ollama pull gemma3:12b
   ```
3. Make sure Ollama is running on http://localhost:11434/

## Project Structure

- `ConsoleApp/` - Contains the main application code
  - `Program.cs` - Main program that sends an image to Ollama and processes the response
  - `cars.jpg` - Sample image for analysis

## How It Works

The application:
1. Connects to a locally running Ollama instance
2. Loads an image file (`cars.jpg`)
3. Sends a prompt asking "How many cars are in the picture?" along with the image
4. Receives and displays the AI model's response

## Dependencies

- `Microsoft.Extensions.AI` (v9.7.0) - Microsoft's AI extensions for .NET
- `OllamaSharp` (v5.2.9) - .NET client library for Ollama

## Running the Application

```bash
cd ConsoleApp
dotnet run
```

## Example Output

```
Result: There are 3 cars visible in the picture.
```

## Customization

You can modify the following in `Program.cs`:
- `ollamaUrl` - URL of your Ollama instance
- `modelId` - The model to use (must be a multimodal model)
- `prompt` - The question to ask about the image
- Image file path - Change to analyze different images
