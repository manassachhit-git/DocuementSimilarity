using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DocumentVectors
{
    public static class ExtractFromUrl
    {
        public static string ExtractTextFromUrl(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            return doc.DocumentNode.SelectSingleNode("//body").InnerText;
        }
    }

    public static class ExtractFromDocument
    {
        /// <summary>
        /// Extract text documents from path
        /// </summary>
        /// <returns></returns>
        public static string ExtractTextDocument(string path)
        {
            var text = System.IO.File.ReadAllText(path);
            return text;
        }

        /// <summary>
        /// Extract any kind of documents from path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ExtractFromDocumentPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            // Get file extension
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // Handle text-based files directly
            string[] textExtensions = { ".txt", ".cs", ".json", ".java", ".py", ".xml", ".html", ".md", ".yml", ".yaml" };

            if (textExtensions.Contains(extension))
            {
                return File.ReadAllText(filePath);  // simple read
            }

            // Optionally: detect binary files and skip
            // You can inspect the first few bytes to see if it's a binary
            using (var stream = File.OpenRead(filePath))
            {
                int b;
                while ((b = stream.ReadByte()) != -1)
                {
                    if (b == 0) // Null byte → binary
                        throw new InvalidOperationException($"The file {filePath} seems to be binary and cannot be read as text.");
                }
            }

            // If not binary, just read as text (default fallback)
            return File.ReadAllText(filePath);
        }
    }


}
