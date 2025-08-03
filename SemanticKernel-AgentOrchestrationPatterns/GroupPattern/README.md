# Group Chat Orchestration Pattern

The Group Chat Orchestration pattern enables multiple AI agents to collaborate in a conversational manner, similar to a team meeting where each member contributes their expertise. This pattern is perfect for complex tasks requiring multiple perspectives and iterative refinement.

## What is Group Chat Orchestration?

Group Chat Orchestration allows multiple agents to:
- **Participate in discussions** where each agent contributes based on their expertise
- **Build upon each other's work** in an iterative manner
- **Request user input** when human approval or guidance is needed
- **Follow conversation management rules** to ensure productive collaboration

## Architecture Overview

This implementation demonstrates a **software development workflow** with:

### Agents
1. **SoftwareEngineerAgent**: Writes code implementations
2. **QATesterAgent**: Tests and validates functionality
3. **CodeReviewAgent**: Reviews code quality and best practices

### Chat Managers
- **RoundRobinGroupChatManager**: Basic turn-taking between agents
- **SmartRoundRobinGroupChatManager**: Intelligent manager that requests user input when approval is mentioned

## How It Works

1. **User provides task** → SoftwareEngineerAgent creates initial implementation
2. **QATesterAgent** analyzes the code for functionality and edge cases
3. **CodeReviewAgent** reviews code quality and best practices
4. **Agents iterate** based on feedback from each other
5. **User input requested** when QA agent mentions "approve"
6. **Process continues** until maximum iterations or completion

## Example Workflow

### Input
```
Write a C# method to calculate the factorial of a number
```

### Agent Collaboration Flow
1. **SoftwareEngineerAgent** → Writes factorial method
2. **QATesterAgent** → Tests for edge cases (negative numbers, zero, large values)
3. **CodeReviewAgent** → Reviews code style, performance, documentation
4. **QATesterAgent** → "I approve this implementation for release"
5. **System** → Requests user input for final approval
6. **User** → Provides final decision

## Key Features

- **Collaborative Discussion**: Agents build upon each other's contributions
- **Role-Based Expertise**: Each agent focuses on their specialized area
- **Intelligent User Interaction**: Smart manager detects when human input is needed
- **Iterative Refinement**: Code improves through multiple review cycles
- **Conversation History**: Complete chat history is maintained and logged
- **Configurable Limits**: Maximum iteration count prevents infinite loops

## Configuration

Create `appsettings.json` with your API keys:

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

## Running the Example

```bash
dotnet run --project GroupPattern
```

The application will:
1. Start the group chat with the predefined task
2. Show each agent's contribution in the console
3. Request user input when the QA agent approves the code
4. Display the complete conversation history

## Chat Manager Types

### RoundRobinGroupChatManager
- **Simple turn-taking**: Each agent gets a turn in order
- **Fixed sequence**: Predictable agent participation
- **Basic orchestration**: No intelligent decision-making

### SmartRoundRobinGroupChatManager (Used in this example)
- **Intelligent user input**: Detects when approval is mentioned
- **Context-aware**: Makes decisions based on conversation content
- **Flexible interaction**: Adapts to conversation flow

## When to Use Group Chat Orchestration

✅ **Perfect for**:
- Code review and development workflows
- Creative brainstorming sessions
- Multi-expert consultation scenarios
- Iterative problem-solving tasks
- Quality assurance processes

❌ **Not ideal for**:
- Simple linear workflows (use Sequential instead)
- Independent parallel tasks (use Concurrent instead)
- Customer support routing (use HandOff instead)

## Customization Options

### Adding New Agents
```csharp
ChatCompletionAgent newAgent = new ChatCompletionAgent
{
    Name = "YourAgentName",
    Description = "Agent description",
    Instructions = "Detailed instructions for the agent",
    Kernel = kernel.Clone()
};
```

### Custom Chat Manager
```csharp
sealed class CustomGroupChatManager : RoundRobinGroupChatManager
{
    public override ValueTask<GroupChatManagerResult<bool>> ShouldRequestUserInput(
        ChatHistory history, CancellationToken cancellationToken = default)
    {
        // Your custom logic here
        return ValueTask.FromResult(new GroupChatManagerResult<bool>(false));
    }
}
```

## Best Practices

1. **Clear Agent Roles**: Define specific responsibilities for each agent
2. **Limit Iterations**: Set MaximumInvocationCount to prevent infinite loops
3. **Smart User Input**: Use intelligent triggers for human interaction
4. **Conversation History**: Log and analyze the complete discussion
5. **Agent Instructions**: Provide detailed, role-specific instructions

## Extending the Pattern

You can enhance this pattern by:
1. **Adding domain experts** (Security, Performance, Documentation agents)
2. **Implementing custom managers** with sophisticated decision logic
3. **Integrating external tools** and plugins for each agent
4. **Creating specialized workflows** for different development phases