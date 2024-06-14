namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// The severity of an <see cref="EPubLog"/>.
    /// </summary>
    public enum EPubLogSeverity
    {
        /// <summary>
        /// Any information about an ePub file that is not a warning or error.
        /// </summary>
        Informational = 0,

        /// <summary>
        /// An aspect of the book is not compliant with the ePub specification, but the book is likely to be renderable by most reading systems.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// A major problem that will almost certainly prevent successful ePub rendering.
        /// </summary>
        Error = 2,

        /// <summary>
        /// A severe problem with the ePub file itself prevents further checking/reading.
        /// </summary>
        Fatal = 3
    }
}
