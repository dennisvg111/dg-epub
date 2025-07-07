using YamlDotNet.Serialization;

namespace DG.Epub.CodeGeneration.Models.Schema;

public class StringSchema : BaseSchema
{
    public string Default { get; set; }
    public string Fixed { get; set; }

    [YamlMember(Alias = "expected_value", ApplyNamingConventions = false)]
    public string ExpectedValue { get; set; }

    [YamlMember(Alias = "xml_name", ApplyNamingConventions = false)]
    public string XmlName { get; set; }

    public List<string> Enum { get; set; }

    public override bool TryGetXmlName(out string name)
    {
        name = XmlName;
        return !string.IsNullOrEmpty(name);
    }

    public override string GetTypename()
    {
        return "string";
    }
}
