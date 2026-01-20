using Microsoft.Extensions.AI;

namespace AgentFramework.SqlChat;

public static class UsageDetailsExtensions
{
    private const string ReasonTokenCountKey = "OutputTokenDetails.ReasoningTokenCount";
    public static long? GetOutputTokensUsedForReasoning(this UsageDetails? usageDetails)
    {
        if (usageDetails?.AdditionalCounts?.TryGetValue(ReasonTokenCountKey, out long reasonTokenCount) ?? false)
        {
            return reasonTokenCount;
        }
        return null;
    }
}