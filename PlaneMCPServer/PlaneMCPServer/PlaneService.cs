using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaneMCPServer;

public class PlaneService(IHttpClientFactory httpClientFactory,
    ILogger<PlaneService> logger,
    string baseUrl,
    string workspace,
    string projectId,
    string apiKey)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _baseUrl = baseUrl;
    private readonly string _workspace = workspace;
    private readonly string _projectId = projectId;
    private readonly string _apiKey = apiKey;
    private readonly ILogger<PlaneService> _logger = logger;
    public async Task<string> GetProjectDetailsAsync()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        _logger.LogInformation("Fetching project details from Plane API.");       
        var url = $"{_baseUrl}api/v1/workspaces/{_workspace}/projects/{_projectId}/states";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CreateWorkItemAsync(string name, string descriptionHtml, string stateId )
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

        var url = $"{_baseUrl}api/v1/workspaces/{_workspace}/projects/{_projectId}/work-items/";
        var payload = new
        {
            name = name,
            description_html = descriptionHtml,
            state_id = stateId
        };

        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}