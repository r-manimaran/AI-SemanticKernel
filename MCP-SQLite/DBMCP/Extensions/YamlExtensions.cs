using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMCP.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class YamlExtensions
{
    public static string ToYaml(this object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        try
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();
                
            return serializer.Serialize(obj);
        }
        catch (Exception ex)
        {
            return $"Error serializing to YAML: {ex.Message}\nObject: {System.Text.Json.JsonSerializer.Serialize(obj)}";
        }
    }
    
}
