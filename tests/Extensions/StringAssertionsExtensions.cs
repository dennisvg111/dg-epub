using FluentAssertions;
using FluentAssertions.Primitives;
using System.Text.RegularExpressions;

namespace DG.Epub.Tests.Extensions
{
    public static class StringAssertionsExtensions
    {
        private static readonly Regex _whitespace = new Regex(@"\s+");
        public static AndConstraint<StringAssertions> BeSameIgnoringWhitespace(this ReferenceTypeAssertions<string, StringAssertions> assertions, string expected, string because = "", params object[] becauseArgs)
        {
            return assertions.Match(v => string.Equals(_whitespace.Replace(v, ""), _whitespace.Replace(expected, "")), because, becauseArgs);
        }
    }
}
