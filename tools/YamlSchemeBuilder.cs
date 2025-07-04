using DG.Epub.CodeGeneration.Models.Schema;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DG.Epub.CodeGeneration;

public static class YamlSchemeBuilder
{
    private static IDeserializer BuildDeserializer()
    {
        return new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .WithTypeDiscriminatingNodeDeserializer(o =>
            {
                o.AddTypeDiscriminator(new BaseSchemaTypeDiscriminator());
            })
            .Build();
    }

    public static Dictionary<string, BaseSchema> BuildSchema(string yaml)
    {
        var deserializer = BuildDeserializer();

        // Step 1: Load the whole schema map
        var schemaMap = deserializer.Deserialize<Dictionary<string, BaseSchema>>(yaml);

        // TODO: throw exception if unnamed object types exist, or add them to scheme map with generated name.
    }
}
