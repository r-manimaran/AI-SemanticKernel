# Semantic Kernel Agent Orchestration Patterns

This project demonstrates different agent orchestration patterns using Microsoft Semantic Kernel, designed to help beginners understand how multiple AI agents can work together to solve complex problems.

## What are Agent Orchestration Patterns?

Agent orchestration patterns define how multiple AI agents collaborate to complete tasks. Think of it like a team of specialists working together - each agent has a specific role and expertise.

## Project Structure

- **BaseKernel**: Shared library containing kernel factory and configuration for both Azure OpenAI and OpenAI
- **SequentialPattern**: Agents work one after another in a chain
- **ConcurrentPattern**: Multiple agents work simultaneously on the same task

## Pattern Comparison

### Sequential Pattern (Chain of Agents)
**Use Case**: Email composition workflow
- **How it works**: Agents execute one after another, each building on the previous agent's output
- **Example Flow**: 
  1. Intent Agent → Understands what user wants
  2. Email Agent → Writes email based on intent
  3. Tone Agent → Reviews and adjusts tone

**When to use**: When you need a step-by-step process where each step depends on the previous one.

### Concurrent Pattern (Parallel Agents)
**Use Case**: Movie recommendation system
- **How it works**: Multiple agents work simultaneously on the same input, each providing their specialized perspective
- **Example Flow**: All agents receive "Suggest a Tamil movie" and respond simultaneously:
  - Action Fan Agent → Suggests action movies
  - Romance Fan Agent → Suggests romantic comedies
  - Classic Fan Agent → Suggests timeless classics
  - Music Fan Agent → Suggests movies with great soundtracks
  - Thriller Fan Agent → Suggests spine-chilling thrillers

**When to use**: When you want multiple perspectives or recommendations on the same topic.

## Configuration

Both patterns use the same configuration. Create `appsettings.json` in each project folder:

```json
{
  "OpenAIConfig": {
    "DeploymentOrModelId": "gpt-4o",
    "ApiKey": "your-openai-api-key"
  },
  "AzureOpenAIConfig": {
    "Endpoint": "https://your-endpoint.openai.azure.com/",
    "ApiKey": "your-azure-api-key",
    "DeploymentOrModelId": "gpt-4o-mini"
  }
}
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- OpenAI API key OR Azure OpenAI service
- Visual Studio or VS Code

### Running the Examples

1. **Clone and Setup**
   ```bash
   git clone <repository-url>
   cd SemanticKernel-AgentOrchestrationPatterns
   ```

2. **Configure API Keys**
   - Add your API keys to `appsettings.json` in each pattern folder
   - Or use user secrets: `dotnet user-secrets set "OpenAIConfig:ApiKey" "your-key"`

3. **Run Sequential Pattern**
   ```bash
   dotnet run --project SequentialPattern
   ```
   *Input*: "Thank a friend and confirm attendance at a birthday party"
   *Output*: A polite email with proper tone

4. **Run Concurrent Pattern**
   ```bash
   dotnet run --project ConcurrentPattern
   ```
   *Input*: "Suggest a good Tamil movie to watch for this weekend"
   *Output*: Multiple movie recommendations from different genres

## Key Concepts for Beginners

### What is a Semantic Kernel?
A framework that lets you integrate AI models (like GPT) into your applications easily.

### What is an Agent?
An AI assistant with specific instructions and personality. Each agent is like hiring a specialist for a particular job.

### What is Orchestration?
Coordinating multiple agents to work together efficiently, like a conductor leading an orchestra.

### Dependency Injection (DI)
A design pattern that makes code more testable and maintainable by providing dependencies from outside rather than creating them inside classes.

## Features

- ✅ **Beginner-Friendly**: Clear examples with detailed explanations
- ✅ **Multiple AI Providers**: Support for both OpenAI and Azure OpenAI
- ✅ **Proper Logging**: Uses Microsoft.Extensions.Logging for better debugging
- ✅ **Dependency Injection**: Modern .NET practices with DI container
- ✅ **Configuration Management**: JSON config with user secrets support
- ✅ **Real-World Examples**: Practical use cases you can adapt

## Dependencies

- .NET 9.0
- Microsoft.SemanticKernel 1.61.0
- Microsoft.SemanticKernel.Agents (Preview packages)
- Microsoft.Extensions.Hosting 9.0.7
- Microsoft.Extensions.Logging

## Next Steps

After running these examples, try:
1. Modifying agent instructions to change their behavior
2. Adding new agents to the concurrent pattern
3. Creating your own orchestration patterns
4. Experimenting with different AI models

## Troubleshooting

**Common Issues:**
- **API Key Error**: Ensure your API keys are correctly set in configuration
- **Model Not Found**: Check if your model name matches what's available in your AI service
- **Rate Limits**: If you hit rate limits, add delays between requests

![alt text](image.png)