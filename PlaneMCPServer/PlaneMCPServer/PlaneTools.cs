using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace PlaneMCPServer;

[McpServerToolType]
public class PlaneTools
{
    [McpServerTool, Description("Get all possible statuses that a work item could be created in. These statuses denote where a work item would be in a typical kanban flow. The state ids returned can be used with other tools such as creating work items.")]
    public static async Task<string> GetAllWorkItemStatuses(PlaneService planeService)
    {
        try
        {
            var statuses = await planeService.GetProjectDetailsAsync();
            return JsonSerializer.Serialize(statuses, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            var err = new
            {
                tool = "GetAllWorkItemStatuses",
                error = ex.Message,
                details = ex.ToString()
            };
            return JsonSerializer.Serialize(err, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    [McpServerTool, Description("Create a new work item in the Plane project management system with the specified name, description, and status.")]
    public static async Task<string> CreateWorkItem(
        PlaneService planeService,
        [Description("The name of the work item to create.")] string name,
        [Description("The description of the work item to create, in HTML format.")] string descriptionHtml,
        [Description("The state ID representing the status to create the work item in. Use the Get All Work Item Statuses tool to find valid state IDs.")] string stateId)
    {
        try
        {
            var result = await planeService.CreateWorkItemAsync(name, descriptionHtml, stateId);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            var err = new
            {
                tool = "CreateWorkItem",
                error = ex.Message,
                details = ex.ToString()
            };
            return JsonSerializer.Serialize(err, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
