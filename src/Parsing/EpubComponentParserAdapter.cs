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
    public bool TryAddDataToBook(EpubBook book, ZipArchive zip, IEpubLogWriter logWriter)
    {
        if (!parser.TryParse(zip, logWriter, out TComponent? data))
        {
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
