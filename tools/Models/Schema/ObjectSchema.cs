namespace DG.Epub.CodeGeneration.Models.Schema;

public class ObjectSchema : BaseSchema
{
    public Dictionary<string, BaseSchema> Properties { get; set; }
    public Dictionary<string, BaseSchema> Attributes { get; set; }

    public override string GetTypename()
    {
        throw new NotSupportedException("Anonymous object types are not yet supported.");
    }
}
