namespace DG.Epub.CodeGeneration.Models.Schema;

public abstract class BaseSchema
{
    public string Type { get; set; }

    public virtual BaseSchema Resolve(Dictionary<string, BaseSchema> schemaMap)
        => this;

    public virtual bool TryGetXmlName(out string name)
    {
        name = null;
        return false;
    }
}