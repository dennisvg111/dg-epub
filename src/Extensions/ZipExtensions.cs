using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

namespace DG.Epub.Extensions
{
    public static class ZipExtensions
    {
        private static readonly XmlReaderSettings _xmlReaderSettings = new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Ignore
        };
        private static readonly IReadOnlyCollection<Func<string, string>> _entryNameTransformations = new Func<string, string>[]
        {
            (n) => n.Replace("\\", "/"),
            (n) => n.Replace("/", "\\"),
            (n) => "/" + n.Replace("\\", "/"),
            (n) => "\\" + n.Replace("/", "\\"),
            (n) => Uri.UnescapeDataString(n),
            (n) => Uri.UnescapeDataString(n.Replace("\\", "/")),
            (n) => Uri.UnescapeDataString(n.Replace("/", "\\")),
            (n) => Uri.UnescapeDataString("/" + n.Replace("\\", "/")),
            (n) => Uri.UnescapeDataString("\\" + n.Replace("/", "\\"))
        };

        /// <summary>
        /// Searches for an entry based on a path, using different combinations of path seperators and leading slashes to try to find the entry.
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="entryName"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool TryFindEntry(this ZipArchive archive, string entryName, out ZipArchiveEntry entry)
        {
            if (entryName.StartsWith("/") || entryName.StartsWith("\\"))
            {
                entryName = entryName.Substring(1);
            }

            entry = archive.GetEntry(entryName);
            if (entry != null)
            {
                return true;
            }

            HashSet<string> triedNames = new HashSet<string>()
            {
                entryName
            };
            foreach (var transformation in _entryNameTransformations)
            {
                var transformedName = transformation(entryName);
                if (triedNames.Contains(transformedName))
                {
                    continue;
                }

                entry = archive.GetEntry(transformedName);
                if (entry != null)
                {
                    return true;
                }
                triedNames.Add(transformedName);
            }

            return false;
        }

        public static XDocument GetXml(this ZipArchiveEntry entry)
        {
            using (var stream = entry.Open())
            using (var reader = XmlReader.Create(stream, _xmlReaderSettings))
            {
                return XDocument.Load(reader);
            }
        }
    }
}
