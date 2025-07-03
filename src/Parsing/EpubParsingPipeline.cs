using DG.Epub.Logging;
using DG.Epub.Parsing.Standard;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="EpubParsingPipeline"/> class with the specified list of parsing steps.
    /// </summary>
    /// <remarks>This pipeline processes EPUB files by applying the specified parsing steps in the order they appear in the list.</remarks>
    /// <param name="parsers">A read-only list of <see cref="IEpubParsingPipelineStep"/> instances that define the steps for parsing EPUB files. Each step in the pipeline is executed sequentially.</param>
    public EpubParsingPipeline(IReadOnlyList<IEpubParsingPipelineStep> parsers)
    {
        _parsers = parsers;
    }

    /// <summary>
    /// Parses an EPUB file from the provided stream and returns the resulting book along with parsing logs.
    /// </summary>
    /// <remarks>This method processes the EPUB file using a pipeline of parsers.
    /// If a parser encounters a fatal error, the parsing process stops early, and the result will include the partial book data and logs up to the point of failure.
    /// </remarks>
    /// <param name="s">The input stream containing the EPUB file. The stream must support reading and must not be null.</param>
    /// <param name="minimumLogLevel">The minimum log level to include in the parsing logs. Defaults to <see langword="EPubLogLevel.Informational"/>.</param>
    /// <returns>An <see cref="EpubComponentResult{EpubBook}"/> containing the parsed <see cref="EpubBook"/> and a collection of logs generated during the parsing process.</returns>
    public EpubComponentResult<EpubBook> Parse(Stream s, EpubLogLevel minimumLogLevel = EpubLogLevel.Informational)
    {
        var logs = new EpubLogCollection(minimumLogLevel);
        logs.AddDebug($"Starting EPUB parsing pipeline using {_parsers.Count} parsers.");

        var book = new EpubBook();

        using (var zip = new ZipArchive(s, ZipArchiveMode.Read, false, null))
        {
            foreach (var parser in _parsers)
            {
                logs.AddDebug($"Running parser {parser.ParserName}.");

                var isParseSuccess = parser.TryAddDataToBook(book, zip, logs);

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

    /// <summary>
    /// Adds a new <paramref name="componentParser"/> to the pipeline and returns a new instance of the pipeline with the updated configuration.
    /// </summary>
    /// <remarks>This method does not modify the current pipeline instance. Instead, it creates a new pipeline instance with the specified parser added to the sequence of parsing steps.</remarks>
    /// <typeparam name="TComponent">The type of the component to be parsed by the specified parser.</typeparam>
    /// <param name="componentParser">The parser responsible for parsing the specified component type. Cannot be <see langword="null"/>.</param>
    /// <returns>A new <see cref="EpubParsingPipeline"/> instance that includes the specified parser.</returns>
    public EpubParsingPipeline ThenWith<TComponent>(IEpubComponentParser<TComponent> componentParser)
    {
        var wrappedParser = new EpubComponentParserAdapter<TComponent>(componentParser);
        var parsersCopy = new List<IEpubParsingPipelineStep>(_parsers);
        parsersCopy.Add(wrappedParser);
        return new EpubParsingPipeline(parsersCopy);
    }

    /// <summary>
    /// Combines the current <see cref="EpubParsingPipeline"/> with another <paramref name="pipeline"/>, creating a new pipeline that includes all parsing steps from both.
    /// </summary>
    /// <remarks>
    /// <para>The returned pipeline includes all parsing steps from the current pipeline followed by the steps from the specified <paramref name="pipeline"/>.</para>
    /// <para>This method does not modify the original pipelines.</para>
    /// </remarks>
    /// <param name="pipeline">The pipeline whose parsing steps will be appended to the current pipeline.</param>
    /// <returns>A new <see cref="EpubParsingPipeline"/> instance containing the combined parsing steps of both pipelines.</returns>
    public EpubParsingPipeline ThenWith(EpubParsingPipeline pipeline)
    {
        var parsersCopy = new List<IEpubParsingPipelineStep>(_parsers);
        parsersCopy.AddRange(pipeline._parsers);
        return new EpubParsingPipeline(parsersCopy);
    }

    /// <summary>
    /// Starts a new EPUB parsing pipeline with the specified <paramref name="componentParser"/>.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component to be parsed.</typeparam>
    /// <param name="componentParser">The parser responsible for parsing the specified component.</param>
    /// <returns>A new instance of <see cref="EpubParsingPipeline"/> initialized with the specified parser.</returns>
    public static EpubParsingPipeline StartWith<TComponent>(IEpubComponentParser<TComponent> componentParser)
    {
        var wrappedParser = new EpubComponentParserAdapter<TComponent>(componentParser);
        return new EpubParsingPipeline([wrappedParser]);
    }

    /// <summary>
    /// Initializes a new <see cref="EpubParsingPipeline"/> instance using the specified pipeline as a starting point.
    /// </summary>
    /// <param name="pipeline">The existing <see cref="EpubParsingPipeline"/> to use as the basis for the new pipeline. Cannot be null.</param>
    /// <returns>A new instance of <see cref="EpubParsingPipeline"/> initialized with the parsers from the specified pipeline.</returns>
    public static EpubParsingPipeline StartWith(EpubParsingPipeline pipeline)
    {
        return new EpubParsingPipeline(pipeline._parsers);
    }

    /// <summary>
    /// Gets a default EPUB parsing pipeline configured with standard parsers.
    /// </summary>
    public static EpubParsingPipeline Default =>
        StartWith(new MimetypeParser())
        .ThenWith(new ContainerParser());
}
