using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
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

        public static bool TryGetEntry(this ZipArchive archive, string entryName, out ZipArchiveEntry entry)
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

            var namesToTry = new List<string>
            {
                "/" + entryName,
                "\\" + entryName,

                // Manifest href's can be url encoded even though actual filenames are not.
                Uri.UnescapeDataString(entryName)
            };

            foreach (var newName in new[]
            {
                    entryName.Replace("\\", "/"),
                    entryName.Replace("/", "\\")
                }.Where(newName => newName != entryName))
            {
                namesToTry.Add(newName);
                namesToTry.Add(Uri.UnescapeDataString(newName));
            }

            foreach (var newName in namesToTry)
            {
                entry = archive.GetEntry(newName);
                if (entry != null)
                {
                    return true;
                }
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
