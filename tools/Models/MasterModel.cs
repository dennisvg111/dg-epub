using DG.Epub.CodeGeneration.Models.Schema;

namespace DG.Epub.CodeGeneration.Models;

public class MasterModel
{
    public string Namespace { get; set; }
    public string FileName { get; set; }

    public Dictionary<string, BaseSchema> Schema { get; set; }
}