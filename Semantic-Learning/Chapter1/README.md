# Semantic Kernel Learning

A .NET 9 console application demonstrating Microsoft Semantic Kernel integration with OpenAI and Azure OpenAI services.

## Project Structure

- **Chapter1/**: Basic Semantic Kernel implementation
  - `Program.cs`: Main application with OpenAI and Azure OpenAI chat completion examples
  - `OpenAIConfig.cs`: Configuration class for OpenAI API settings
  - `AzureOpenAIConfig.cs`: Configuration class for Azure OpenAI settings
  - `appsettings.json`: Configuration file for API keys and endpoints

## Features

- OpenAI GPT integration using Semantic Kernel
- Azure OpenAI service support
- Configuration management with user secrets
- Chat completion functionality

## Setup

1. **Install dependencies**:
   ```bash
   dotnet restore
   ```

2. **Configure API keys** using user secrets:
   ```bash
   dotnet user-secrets set "LLM:ApiKey" "your-openai-api-key"
   dotnet user-secrets set "AzureOpenAI:ApiKey" "your-azure-openai-key"
   dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-endpoint.openai.azure.com/"
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

## Configuration

The application supports both OpenAI and Azure OpenAI through configuration in `appsettings.json` and user secrets.

### Required Settings
- `LLM:ApiKey`: OpenAI API key
- `LLM:DeploymentOrModelId`: Model ID (default: gpt-35-turbo)
- `AzureOpenAI:Endpoint`: Azure OpenAI endpoint
- `AzureOpenAI:ApiKey`: Azure OpenAI API key
- `AzureOpenAI:DeploymentNameModelId`: Azure deployment name

## Dependencies

- Microsoft.SemanticKernel (1.60.0)
- Microsoft.Extensions.Configuration (9.0.7)
- Microsoft.Extensions.Configuration.UserSecrets (6.0.1)