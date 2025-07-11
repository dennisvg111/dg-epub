using DG.Epub.CodeGeneration.Models.Schema;

namespace DG.Epub.CodeGeneration.Models;

public static class SchemaHelpers
{
    public static string ToPascalCase(string snakeCase)
    {
        return string.Concat(snakeCase.Split('_').Select(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase));
    }

    public static string GetCSharpName(KeyValuePair<string, BaseSchema> node)
    {
        return ToPascalCase(node.Key);
    }

    public static string GetXmlName(KeyValuePair<string, BaseSchema> node)
    {
        if (node.Value.TryGetXmlName(out string name))
        {
            return name;
        }
        return node.Key;
    }
}
