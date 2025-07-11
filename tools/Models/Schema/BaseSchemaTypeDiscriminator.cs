using DG.Epub.CodeGeneration.Models.Schema;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.BufferedDeserialization.TypeDiscriminators;

public class BaseSchemaTypeDiscriminator : ITypeDiscriminator
{
    private static readonly Dictionary<string, Type> typeLookup = new Dictionary<string, Type>() {
                { "array", typeof(ArraySchema) },
                { "string" , typeof(StringSchema) },
                { "object" , typeof(ObjectSchema) },
            };

    public Type BaseType => typeof(BaseSchema);

    public bool TryDiscriminate(IParser buffer, out Type suggestedType)
    {
        if (buffer.TryFindMappingEntry(s => s.Value == "type" || s.Value == "$ref", out Scalar key, out ParsingEvent e) && e is Scalar valueScalar)
        {
            if (key.Value == "$ref")
            {
                suggestedType = typeof(RefSchema);
                return true;
            }

            if (typeLookup.TryGetValue(valueScalar.Value, out var targetType))
            {
                suggestedType = targetType;
                return true;
            }

            throw new InvalidOperationException($"Unknown schema type: '{valueScalar.Value}'");
        }

        suggestedType = null;
        return false;
    }
}