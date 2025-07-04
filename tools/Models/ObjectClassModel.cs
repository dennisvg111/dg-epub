using DG.Epub.CodeGeneration.Models.Schema;

namespace DG.Epub.CodeGeneration.Models;
public class ObjectClassModel
{
    public string Name { get; set; }
    public ObjectSchema ClassSchema { get; set; }
    public Dictionary<string, BaseSchema> SchemaMap { get; set; }
}
