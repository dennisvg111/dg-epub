namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// The level or severity of an <see cref="EPubLog"/>.
    /// </summary>
    public enum EPubLogLevel
    {
        /// <summary>
        /// Developer-oriented debug information about the parsing process.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Any information about an ePub file that is not a warning or error, and not specifically debugging information for developers.
        /// </summary>
        Informational = 1,

        /// <summary>
        /// An aspect of the book is not compliant with the ePub specification, but the book is likely to be renderable by most reading systems.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// A major problem that will almost certainly prevent successful ePub rendering.
        /// </summary>
        Error = 3,

        /// <summary>
        /// A severe problem with the ePub file itself prevents further checking/reading.
        /// </summary>
        Fatal = 4
    }
}
