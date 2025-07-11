using YamlDotNet.Serialization;

namespace DG.Epub.CodeGeneration.Models.Schema;

public class RefSchema : BaseSchema
{

    [YamlMember(Alias = "$ref")]
    public string Ref { get; set; }

    public override BaseSchema Resolve(Dictionary<string, BaseSchema> schemaMap)
    {
        if (string.IsNullOrEmpty(Ref))
            throw new InvalidOperationException("Missing $ref");

        var key = Ref.TrimStart('#');
        if (!schemaMap.TryGetValue(key, out var resolved))
            throw new KeyNotFoundException($"$ref '{Ref}' not found in schema map.");

        return resolved.Resolve(schemaMap); // Recursively resolve
    }

    public override string GetTypename()
    {
        return SchemaHelpers.ToPascalCase(Ref.TrimStart('#'));
    }
}
