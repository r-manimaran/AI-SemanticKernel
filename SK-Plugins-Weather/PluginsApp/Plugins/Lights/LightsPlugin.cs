using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginsApp.Plugins.Lights;

public class LightsPlugin
{
    public readonly List<LightModel> lights = new()
    {
        new LightModel() {Id=1, Name="Table Lamp", IsOn=false, Brightness=100, Hex="FF0000"},
        new LightModel() {Id=2, Name="Porch light", IsOn=false, Brightness=50, Hex="00FF00"},
        new LightModel() {Id=3, Name="Chandelier", IsOn=true, Brightness=75, Hex="0000FF"}
    };
    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    public async Task<List<LightModel>> GetLightsAsync()
    {
        return lights;
    }

    [KernelFunction("get_state")]
    [Description("Gets the state of a particular light")]
    public async Task<LightModel?> GetStateAsync([Description("The ID of the light")] int id)
    {
        return lights.FirstOrDefault(light=>light.Id == id);
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the light")]
    public async Task<LightModel?> ChangeStateAsync(int id, LightModel newLight)
    {
        var light = lights.FirstOrDefault(light => light.Id == id);

        if(light == null)
        {
            return null;
        }

        // Update the light
        light.Brightness = newLight.Brightness;
        light.Hex = newLight.Hex;
        light.IsOn = newLight.IsOn;
        return light;
    }
}

public class LightModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string  Name { get; set; }
    [JsonPropertyName("is_on")]
    public bool? IsOn { get; set; }
    [JsonPropertyName("brightness")]
    public byte? Brightness { get; set; }
    [JsonPropertyName("hex")]
    public string? Hex { get; set; }
}
