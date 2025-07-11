using DG.Epub.Parsing;
using DG.Epub.Stucture;

namespace DG.Epub;

/// <summary>
/// Represents an EPUB book, providing functionality to interact with EPUB file data.
/// </summary>
/// <remarks>
/// <para>This class serves as the main entry point for working with EPUB files.</para>
/// <para>Use the <see cref="EpubParsingPipeline"/> class to create an instance of <see cref="EpubBook"/> from a valid EPUB file.</para>
/// </remarks>
public class EpubBook
{
    /// <summary>
    /// Gets or sets the MIME type file associated with the current EPUB book.
    /// </summary>
    /// <remarks>This should usually be <see cref="MimetypeFile.Default"/>.</remarks>
    public MimetypeFile? MimetypeFile { get; set; }

    /// <summary>
    /// Gets or sets the container file associated with the current EPUB book.
    /// </summary>
    public ContainerFile? ContainerFile { get; set; }

    public PackageDocument? PackageDocument { get; set; }
}