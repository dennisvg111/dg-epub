using DG.Epub.CodeGeneration.Models;
using RazorLight;

namespace DG.Epub.CodeGeneration;

internal class Program
{
    static async Task Main(string[] args)
    {
        var yaml = File.ReadAllText("Example/example.yaml");

        var schemaMap = YamlSchemeBuilder.BuildSchema(yaml);

        var model = new MasterModel()
        {
            FileName = "test.cs",
            Namespace = "DG.Epub.Generated",
            Schema = schemaMap
        };

        var result = await CompileTemplates(model);

        Console.WriteLine(result);
    }

    private static async Task<string> CompileTemplates(MasterModel model)
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(Program)) // or any type in the same assembly
            .UseMemoryCachingProvider()
            .Build();

        return await engine.CompileRenderAsync("Templates.MasterTemplate.cshtml", model);
    }
}
