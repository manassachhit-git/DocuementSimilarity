using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentVectors
{
    public static class FileProcessor
    {
        public static string ExtractAndTokenize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            string content = File.ReadAllText(filePath);
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".txt" or ".md" or ".html" => PreprocessCode.CreateTextTokens(content),
                ".json" or ".yml" or ".yaml" => PreprocessCode.CreateJsonTokens(content),
                ".cs" or ".java" or ".py" or ".cpp" or ".js" => PreprocessCode.CreateCodeTokens(content),
                _ => PreprocessCode.CreateTextTokens(content) // fallback
            };
        }
    }

}
