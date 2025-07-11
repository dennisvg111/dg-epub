using DG.Epub.CodeGeneration.Models;
using RazorLight;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XmlSchemaClassGenerator;

namespace DG.Epub.CodeGeneration;

internal class Program
{
    private const string _outputFolderFlag = "outputFolder";
    private const string _namespacePrefixFlag = "namespacePrefix";


    static async Task Main(string[] args)
    {
        string input = null;
        string outputFolder = null;
        string namespacePrefix = null;

        if (args.Length == 0)
        {
            Console.Write(GetHelp());
            return;
        }
        foreach (var arg in args)
        {
            if (arg.Equals("--help", StringComparison.OrdinalIgnoreCase) || arg.Equals("-h", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write(GetHelp());
                return;
            }
            if (arg.StartsWith($"--{_outputFolderFlag}=", StringComparison.OrdinalIgnoreCase) || arg.StartsWith("-o=", StringComparison.OrdinalIgnoreCase))
            {
                outputFolder = arg.Substring(arg.IndexOf("=") + 1).Trim();
            }
            else if (arg.StartsWith($"--{_namespacePrefixFlag}=", StringComparison.OrdinalIgnoreCase) || arg.StartsWith("-n=", StringComparison.OrdinalIgnoreCase))
            {
                namespacePrefix = arg.Substring(arg.IndexOf("=") + 1).Trim();
            }
            else if (input == null)
            {
                input = arg;
            }
            else
            {
                throw new ArgumentException($"Unexpected argument: '{arg}'");
            }

        }
        var yaml = File.ReadAllText(input);

        var schemaMap = YamlSchemeBuilder.BuildSchema(yaml);

        var xsdDocument = XsdGenerator.GenerateXsd(schemaMap);
        //await WriteXDocument(xsdDocument);
        var xmlSchemaSet = LoadSchemaSetFromXDocument(xsdDocument);

        var generator = new Generator()
        {
            OutputFolder = outputFolder,
            GenerateInterfaces = true,
            GenerateComplexTypesForCollections = false,
            CollectionImplementationType = typeof(List<>),
            CollectionType = typeof(List<>),
            EnableNullableReferenceAttributes = false,
            DataAnnotationMode = DataAnnotationMode.None,
            NamespaceProvider = new Dictionary<NamespaceKey, string>
            {
                { new NamespaceKey("http://www.w3.org/2001/XMLSchema"), "xsd" }
            }
            .ToNamespaceProvider(new GeneratorConfiguration { NamespacePrefix = namespacePrefix }.NamespaceProvider.GenerateNamespace),
        };
        generator.Configuration.SeparateClasses = false;
        generator.Configuration.SeparateNamespaceHierarchy = false;
        generator.Configuration.CommentLanguages.Add("en");
        generator.Configuration.Log = (s) => Console.WriteLine("[XmlSchemaClassGenerator] " + s);

        generator.Generate(xmlSchemaSet);
    }

    private static string GetHelp()
    {
        return new StringBuilder()
            .AppendLine("Usage:")
            .AppendLine("  dotnet run INPUT [-o=PATH] [-n=PREFIX] [-h|--help]")
            .AppendLine()
            .AppendLine("Options:")
            .AppendLine("  INPUT:                               Path to the input yaml file.")
            .AppendLine($"  --{_outputFolderFlag}=PATH, -o=PATH         Folder to write output files.")
            .AppendLine($"  --{_namespacePrefixFlag}=PREFIX, -n=PREFIX  Namespace prefix to use.")
            .AppendLine("  --help, -h                           Show this help message.")
            .ToString();

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
