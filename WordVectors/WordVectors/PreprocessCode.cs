using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocumentVectors
{
    public static class PreprocessCode
    {

        private static readonly HashSet<string> CodeKeywords = new HashSet<string>
        {
            "if","else","for","while","return","class","def","public","private","static","void",
            "int","string","float","double","new","using","namespace","import","try","catch",
            "bool","true","false","null","const","var"
        };

        //  Natural text / docs (your existing logic)
        public static string CreateTextTokens(string text)
        {
            // Lowercase and remove newlines
            string clean = Regex.Replace(text.ToLowerInvariant(), @"\s+", " ");

            // Remove stop words
            string[] stopWords = { "the", "is", "and", "a", "an", "to", "of", "in", "on", "for" };
            foreach (var sw in stopWords)
                clean = Regex.Replace(clean, $@"\b{sw}\b", " ");

            // Remove duplicate words
            clean = string.Join(" ", clean.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct());

            return clean;
        }

        // Code files (.cs, .java, .py, etc.)
        public static string CreateCodeTokens(string code)
        {
            // 1. Remove comments
            string noComments = Regex.Replace(code, @"//.*?$|/\*.*?\*/", "", RegexOptions.Singleline | RegexOptions.Multiline);

            // 2. Remove string literals
            string noStrings = Regex.Replace(noComments, "\".*?\"", "STR");

            // 3. Tokenize
            var tokens = Regex.Matches(noStrings, @"[A-Za-z_][A-Za-z0-9_]*|\d+|[{}();=+\-*/<>]")
                              .Cast<Match>()
                              .Select(m => m.Value)
                              .ToList();

            // 4. Normalize identifiers
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];
                if (!CodeKeywords.Contains(token) && Regex.IsMatch(token, @"^[A-Za-z_][A-Za-z0-9_]*$"))
                {
                    if (char.IsLower(token[0]))
                        tokens[i] = "VAR"; // likely variable
                    else
                        tokens[i] = token.ToLowerInvariant(); // keep class/method names
                }
                else if (Regex.IsMatch(token, @"^\d+$"))
                {
                    tokens[i] = "NUM";
                }
            }

            return string.Join(" ", tokens);
        }

        // JSON / config files
        public static string CreateJsonTokens(string json)
        {
            try
            {
                var node = JsonNode.Parse(json);
                var flatTokens = new List<string>();
                ProcessJson(node, flatTokens, "");
                return string.Join(" ", flatTokens);
            }
            catch
            {
                // Fallback: raw compact
                return Regex.Replace(json, @"\s+", " ");
            }
        }

        private static void ProcessJson(JsonNode node, List<string> tokens, string prefix)
        {
            if (node is JsonObject obj)
            {
                foreach (var kvp in obj.OrderBy(k => k.Key)) // sort keys for consistency
                {
                    ProcessJson(kvp.Value, tokens, $"{prefix}{kvp.Key}.");
                }
            }
            else if (node is JsonArray arr)
            {
                tokens.Add(prefix + "ARRAY");
                foreach (var item in arr)
                {
                    ProcessJson(item, tokens, prefix);
                }
            }
            else if (node is JsonValue val)
            {
                if (val.TryGetValue(out string s))
                    tokens.Add(prefix + "STR");
                else if (val.TryGetValue(out double d))
                    tokens.Add(prefix + "NUM");
                else if (val.TryGetValue(out bool b))
                    tokens.Add(prefix + b.ToString().ToLowerInvariant());
                else
                    tokens.Add(prefix + "VAL");
            }
        }
    }
}
