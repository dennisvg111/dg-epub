using DG.Epub.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DG.Epub.Parsing;

/// <summary>
/// Represents a pipeline for parsing EPUB files using a sequence of parsers.
/// </summary>
/// <remarks>Parsers are executed in the order they are added to the pipeline, and the parsing process stops if a parser encounters a fatal error.</remarks>
public class EpubParsingPipeline
{
    private readonly IReadOnlyList<IEpubParsingPipelineStep> _parsers;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpubParsingPipeline"/> class.
    /// </summary>
    /// <remarks>This constructor initializes the pipeline with no parsers. Parsers can be added later to process EPUB files.</remarks>
    public EpubParsingPipeline()
    {
        _parsers = Array.Empty<IEpubParsingPipelineStep>();
    }

    private EpubParsingPipeline(IReadOnlyList<IEpubParsingPipelineStep> parsers)
    {
        _parsers = parsers;
    }

    /// <summary>
    /// Parses an EPUB file from the provided stream and returns the resulting book along with parsing logs.
    /// </summary>
    /// <remarks>This method processes the EPUB file using a pipeline of parsers.
    /// If a parser encounters a fatal error, the parsing process stops early, and the result will include the partial book data and logs up to the point of failure.</remarks>
    /// <param name="s">The input stream containing the EPUB file. The stream must support reading and must not be null.</param>
    /// <param name="minimumLogLevel">The minimum log level to include in the parsing logs. Defaults to <see langword="EPubLogLevel.Informational"/>.</param>
    /// <returns>An <see cref="EpubParsingResult{EpubBook}"/> containing the parsed <see cref="EpubBook"/> and a collection of logs generated during the parsing process.</returns>
    public EpubParsingResult<EpubBook> Parse(Stream s, EpubLogLevel minimumLogLevel = EpubLogLevel.Informational)
    {
        var logs = new EpubLogCollectoin(minimumLogLevel);
        logs.AddDebug($"Starting EPUB parsing pipeline using {_parsers.Count} parsers.");

        var book = new EpubBook();

        using (var zip = new ZipArchive(s, ZipArchiveMode.Read, false, null))
        {
            foreach (var parser in _parsers)
            {
                logs.AddDebug($"Running parser {parser.ParserName}.");

                var isParseSuccess = parser.TryAddDataToBook(book, zip, minimumLogLevel, out EpubLogCollectoin stepLogs);

                logs.AddAll(stepLogs);

                if (!isParseSuccess)
                {
                    logs.AddDebug($"Encountered fatal error while executing parser {parser.ParserName}, stopping early.");
                    return EPubParsingResult.Completed(book, logs);
                }
            }

            logs.AddDebug($"Parsing completed successfully.");
            return EPubParsingResult.Completed(book, logs);
        }
    }

    public EpubParsingPipeline ThenWith<TComponent>(IEpubComponentParser<TComponent> parser)
    {
        var wrappedParser = new EpubComponentParserAdapter<TComponent>(parser);
        var parsersCopy = new List<IEpubParsingPipelineStep>(_parsers);
        parsersCopy.Add(wrappedParser);
        return new EpubParsingPipeline(parsersCopy);
    }

    public EpubParsingPipeline ThenWith(EpubParsingPipeline pipeline)
    {
        var parsersCopy = new List<IEpubParsingPipelineStep>(_parsers);
        parsersCopy.AddRange(pipeline._parsers);
        return new EpubParsingPipeline(parsersCopy);
    }

    public static EpubParsingPipeline StartWith<TComponent>(IEpubComponentParser<TComponent> parser)
    {
        var wrappedParser = new EpubComponentParserAdapter<TComponent>(parser);
        return new EpubParsingPipeline([wrappedParser]);
    }

    public static EpubParsingPipeline StartWith(EpubParsingPipeline pipeline)
    {
        return new EpubParsingPipeline(pipeline._parsers);
    }
}
