using DG.Epub.CodeGeneration.Models;
using RazorLight;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XmlSchemaClassGenerator;

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

        var xsdDocument = XsdGenerator.GenerateXsd(schemaMap);
        await WriteXDocument(xsdDocument);
        var xmlSchemaSet = LoadSchemaSetFromXDocument(xsdDocument);

        var generator = new Generator()
        {
            Log = s => Console.WriteLine(s),
            OutputFolder = @"C:\Temp",
            GenerateInterfaces = true,
            GenerateComplexTypesForCollections = false,
            CollectionImplementationType = typeof(List<>),
            CollectionType = typeof(List<>),
            NamespaceProvider = new Dictionary<NamespaceKey, string>
            {
                { new NamespaceKey("http://www.w3.org/2001/XMLSchema"), "xsd" }
            }
            .ToNamespaceProvider(new GeneratorConfiguration { NamespacePrefix = "DG.Epub.Generated" }.NamespaceProvider.GenerateNamespace),
        };
        generator.Configuration.SeparateClasses = false;
        generator.Configuration.SeparateNamespaceHierarchy = false;
        generator.Configuration.CommentLanguages.Add("en");
        generator.Configuration.Log = (s) => Console.WriteLine("[XmlSchemaClassGenerator] " + s);

        generator.Generate(xmlSchemaSet);
    }

    private static async Task WriteXDocument(XDocument document)
    {
        var xws = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true,
            Async = true,
        };

        using (var stream = File.Create(@"C:\Temp\new.xsd"))
        using (XmlWriter xw = XmlWriter.Create(stream, xws))
        {
            await document.SaveAsync(xw, CancellationToken.None);
        }
    }

    private static XmlSchemaSet LoadSchemaSetFromXDocument(XDocument doc)
    {
        var schemaSet = new XmlSchemaSet();
        using (var reader = doc.CreateReader())
        {
            var schema = XmlSchema.Read(reader, (sender, args) =>
            {
                Console.WriteLine($"Schema error: {args.Message}");
            });

            schemaSet.Add(schema);
        }

        schemaSet.Compile();

        Console.WriteLine($"Compiled schema set with {schemaSet.Schemas().Count} schemas.");

        return schemaSet;
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
