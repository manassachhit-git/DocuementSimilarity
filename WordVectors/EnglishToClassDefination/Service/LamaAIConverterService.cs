using EnglishToClassDefinition.SchemaBuilder;
using EnglishToClassDefinition.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Service
{
    public class LamaAIConverterService
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;

        public LamaAIConverterService(string endpointUrl, string apiKey)
        {
            _endpoint = endpointUrl;

            _http = new HttpClient();
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<Dictionary<string, object>> ExtractAsync(string englishText, Type targetType)
        {
            var props = Utility.GetPropertyNames(targetType);

            var schema = ClassSchemaBuilder.Build(targetType);
            var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string prompt = $@"
            You are a strict JSON type corrector and structured data extraction engine using:
            (1) English text input
            (2) C# class schema and property definitions

            STRICT RULES:
            - Output ONLY a pure JSON object — raw format (no backticks, no code fences, no comments).
            - JSON must match EXACTLY the required keys from the provided C# schema.
            - Do NOT add, rename, or infer any properties not present in the schema.
            - Do NOT drop any values that appear in the JSON input or extracted from text.
            - Maintain exact C# schema data types:
                ✔ Convert numeric strings (""32"") → numbers (32)
                ✔ Convert single string → array if schema says array
                ✔ If comma-separated string is given, split into array: ""a,b"" → [""a"",""b""]
            - Complex collections must be a JSON array of objects only.
            - Ignore all .NET metadata fields such as: Capacity, Count, Item[], etc.
            - Fix ONLY type mismatches — preserve content and intent.
            - If a field is missing from input, assign null or schema default if available.
            - Ignore computed properties in C# (e.g. FullName) unless explicitly defined in schema.
            - NO markdown formatting such as ```json or any backticks — pure JSON only.

            TASK:
            Return corrected JSON that fully matches the C# schema.

            TEXT:
            {englishText}

            SCHEMA:
            {schemaJson}

            Required JSON keys: {string.Join(", ", props)}

            Return:
            {{ ""key"": ""value"" }} format only.
            ";

            var payload = new
            {
                model = "meta/llama-4-maverick-17b-128e-instruct-maas",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            string body = JsonSerializer.Serialize(payload);

            var response = await _http.PostAsync(_endpoint,
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                throw new Exception($"LLM error: {response.StatusCode}");

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            string content =
                json.RootElement
                    .GetProperty("choices")[0]!
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()??"{}";

            // Log the raw response for debugging
            Console.WriteLine("Raw AI response:");
            Console.WriteLine(content);

            // Clean up the response: remove code fences and trim whitespace
            content = content.Trim();
            if (content.StartsWith("```"))
            {
                int firstNewline = content.IndexOf('\n');
                int lastFence = content.LastIndexOf("```");
                if (firstNewline >= 0 && lastFence > firstNewline)
                {
                    content = content.Substring(firstNewline + 1, lastFence - firstNewline - 1).Trim();
                }
            }
            content = content.Trim('`');

            // Log the cleaned response
            Console.WriteLine("Cleaned AI response:");
            Console.WriteLine(content);
            // Validate JSON before deserialization
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(content)!;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON parsing error:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
