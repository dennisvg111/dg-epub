namespace DG.Epub.CodeGeneration.Models.Schema;
public class ArraySchema : BaseSchema
{
    public BaseSchema Items { get; set; }

    public override string GetTypename()
    {
        return Items.GetTypename() + "[]";
    }
}
