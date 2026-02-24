using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AgenticProjectDescGenerator;

internal class PluginLoggingFilter(ILogger<PluginLoggingFilter> logger) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, 
                                    Func<FunctionInvocationContext, Task> next)
    {
       // 1. Pre-Execution Logging
        var functionName = context.Function.Name;
        var pluginName = context.Function.PluginName;
        var arguments = JsonSerializer.Serialize(context.Arguments);

        logger.LogInformation("[AGENT CALL] Plugin: {Plugin}, Function: {Function}. Args: {Args}",
            pluginName, functionName, arguments);

        try
        {
            // 2. Execute the actual function
            await next(context);

            // 3. Post-Execution Logging
            var result = context.Result.ToString();
            logger.LogInformation("[AGENT RESULT] {Function} returned: {Result}",
               functionName, result);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[AGENT ERROR] Function {Function} failed.", functionName);
            throw; // Re-throw to let the Agent handle the failure
        }
    }
}
