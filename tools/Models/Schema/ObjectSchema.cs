using YamlDotNet.Serialization;

namespace DG.Epub.CodeGeneration.Models.Schema;

public class ObjectSchema : BaseSchema
{
    [YamlMember(Alias = "xml_type", ApplyNamingConventions = false)]
    public string XmlType { get; set; }

    [YamlMember(Alias = "xml_root", ApplyNamingConventions = false)]
    public string XmlRoot { get; set; }

    [YamlMember(Alias = "xml_namespace", ApplyNamingConventions = false)]
    public string XmlNamespace { get; set; }

    public Dictionary<string, BaseSchema> Properties { get; set; }
    public Dictionary<string, BaseSchema> Attributes { get; set; }

    public override string GetTypename()
    {
        throw new NotSupportedException("Anonymous object types are not yet supported.");
    }
}
