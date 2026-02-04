using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML;

namespace DocumentVectors
{
    public static class PreprocessText
    {
        public static string RemoveWhiteSpaces(string text)
        {
            var replacedText = text.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
            return replacedText;
        }

        public static string ConvertToLower(string text)
        {
            return text.ToLower();
        }

        public static string RemovePunctuation(string text)
        {
            var sb = new StringBuilder();
            foreach (var ch in text)
            {
                if (!char.IsPunctuation(ch))
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        public static string SanitizeText(string text)
        {
            var noWhitespace = RemoveWhiteSpaces(text);
            var lowerCased = ConvertToLower(noWhitespace);
            //var noPunctuation = RemovePunctuation(lowerCased);
            var noDuplicates = RemoveDuplicates(lowerCased);
            var noStopWords = RemoveStopWords(noDuplicates);
            return noStopWords;
        }

        public static string RemoveDuplicates(string text)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var distinctWords = words.Distinct();
            return string.Join(' ', distinctWords);
        }

        public static string RemoveStopWords(string text)
        {
            var stopWords = new HashSet<string> { "the", "and", "a", "of", "in", "to" };
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filteredWords = words.Where(word => !stopWords.Contains(word));
            return string.Join(' ', filteredWords);
        }
    }
}
