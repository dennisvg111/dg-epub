﻿using System.IO;
using Xunit;

namespace DG.Epub.Tests
{
    public class EpubBookTests
    {
        [Fact]
        public void FromStream_Tests()
        {
            using (var exampleStream = File.OpenRead("minimal-v3.epub"))
            {
                var book = EpubBook.FromStream(exampleStream);
            }
        }
    }
}
