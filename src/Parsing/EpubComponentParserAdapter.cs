using DG.Epub.Logging;
using System.IO.Compression;

namespace DG.Epub.Parsing;

internal class EpubComponentParserAdapter<TComponent> : IEpubParsingPipelineStep
{
    private readonly IEpubComponentParser<TComponent> parser;

    public EpubComponentParserAdapter(IEpubComponentParser<TComponent> parser)
    {
        this.parser = parser;
    }

    /// <inheritdoc/>
    public string ParserName => $"{parser.GetType().Name}[{typeof(TComponent).Name}]";

    /// <inheritdoc/>
    public bool TryAddDataToBook(EpubBook book, ZipArchive zip, EpubLogLevel minimumLogLevel, out EpubLogCollectoin logs)
    {
        logs = new EpubLogCollectoin(minimumLogLevel);
        if (!parser.TryParse(zip, logs, out TComponent? data))
        {
            if (logs.HighestSeverity < EpubLogLevel.Fatal)
            {
                logs.AddFatal($"Unknown fatal error encountered while running {ParserName}.");
            }

            return false;
        }

        parser.AddToBook(book, data);

        return true;
    }

    public override string ToString()
    {
        return ParserName;
    }
}
