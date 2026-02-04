using EnglishToClassDefinition.SchemaBuilder;
using EnglishToClassDefinition.Service;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Utilities
{
    /// <summary>
    /// Utility for converting English text to JSON using AI services with schema correction.
    /// </summary>
    public class ConverterUtility
    {
        private readonly LamaAIConverterService? _lamaService;
        private readonly SelfCorrectorLLamaService? _selfCorrectorService;

        /// <summary>
        /// Initializes the utility with either a LamaAIConverterService or SelfCorrectorLLamaService.
        /// </summary>
        /// <param name="lamaService">LamaAIConverterService instance (optional).</param>
        /// <param name="selfCorrectorService">SelfCorrectorLLamaService instance (optional).</param>
        /// <exception cref="ArgumentException">Thrown if both or neither services are provided.</exception>
        public ConverterUtility(
            LamaAIConverterService? lamaService = null,
            SelfCorrectorLLamaService? selfCorrectorService = null)
        {
            if ((lamaService == null && selfCorrectorService == null) ||
                (lamaService != null && selfCorrectorService != null))
            {
                throw new ArgumentException("Provide exactly one service instance.");
            }
            _lamaService = lamaService;
            _selfCorrectorService = selfCorrectorService;
        }

        /// <summary>
        /// Converts English text to JSON using schema correction.
        /// </summary>
        /// <typeparam name="T">Target type for schema.</typeparam>
        /// <param name="text">Input English text.</param>
        /// <returns>Corrected JSON string.</returns>
        public async Task<string> ConvertAsyncWithSelfCorrection<T>(string text)
        {
            if (_selfCorrectorService == null)
                throw new InvalidOperationException("SelfCorrectorLLamaService is required.");

            var schema = ClassSchemaBuilder.Build(typeof(T));
            var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
            var props = Utility.GetPropertyNames(typeof(T));

            string prompt = BuildExtractionPrompt(text, props);
            var extractPayload = new
            {
                model = "meta/llama-4-maverick-17b-128e-instruct-maas",
                messages = new[] { new { role = "user", content = prompt } },
                naturalText = text,
            };

            string firstPass = await _selfCorrectorService.SendAsync(extractPayload);

            string correctionPrompt = BuildCorrectionPrompt(schemaJson, firstPass);
            var correctionPayload = new
            {
                model = "meta/llama-4-maverick-17b-128e-instruct-maas",
                messages = new[] { new { role = "user", content = correctionPrompt } },
                schema,
                modelOutput = JsonSerializer.Deserialize<object>(firstPass)
            };

            string cleaned = await _selfCorrectorService.SendAsync(correctionPayload);
            return Utility.CleanJsonFences(cleaned);
        }

        /// <summary>
        /// Builds the extraction prompt for the AI model.
        /// </summary>
        private static string BuildExtractionPrompt(string text, IEnumerable<string> props)
        {
            return $@"
                You are a structured data extraction engine from the english text, schema and class definition.

                STRICT RULES:
                - Ignore .NET list metadata fields like Capacity, Count, Item[] entirely.
                - Output ONLY a pure JSON object — no explanation or comments.
                - JSON must match exactly the required keys defined in schema.
                - IF a property is a LIST or ARRAY → return it as a JSON ARRAY only.
                - ✔ No ""Count"", ""Capacity"", ""Items"",""Comparer"",""Keys"",""Values"" or other wrapper fields.
                - Complex list values MUST be inside [] as objects.
                - Do not infer, rename or add any properties not in schema.
                - Maintain the exact data types from schema.
                - If a value is missing in the text → assign null or default schema value.
                - If a schema property is an enum:
                    • Output only the enum string value.
                    • Do NOT wrap it inside an object.
                - Attributes must be a plain JSON dictionary (key-value map), with no metadata objects or array-based key-value pairs.
                - For polymorphic properties (oneOf types):
                     • A type is considered VALID ONLY IF all of its REQUIRED properties are explicitly mentioned in the input text.
                     • If none of the oneOf schemas have all required fields present in the text:
                        → Set the polymorphic property value to null.
                - ️Never add default or placeholder values just to satisfy a schema.
                - ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.

                TEXT:
                {text}

                Required JSON keys: {string.Join(", ", props)}

                Return:
                -ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.
                {{ ""key"": ""value"" }} json format only with complex data types.
                ";
        }

        /// <summary>
        /// Builds the correction prompt for the AI model.
        /// </summary>
        private static string BuildCorrectionPrompt(string schemaJson, string firstPass)
        {
            // Fetch strict rules from file
            string strictRules = "";
            string ruleFilePath = @"C:\Users\mbasawatia\source\repos\WordVectors\EnglishToClassDefination\RuleFiles\Roledetails_Rule.txt";
            strictRules = ReadRuleFile(ruleFilePath);

            return $@"
                You are a strict JSON type corrector.

                Given:
                1. The target C# schema:
                {schemaJson}

                2. The JSON objects that needs correction:
                {firstPass}

                3. Strict Rules Files:
                {strictRules}

                Task:
                - Do not use old Data.
                - Fix all mismatches to match the C# schema exactly.
                - If the schema expects an array (e.g. string[]):
                        - If the JSON value contains a comma-separated string, split by \"" , \"" into an array with individual values wrapped in Quotes \""\"".
                        - If the JSON contains a single string, wrap it in an array([\""value\""]).
                -Convert numeric strings(\""32\"") to numbers(32) if required by schema.
                -Do NOT drop values.
                - Do NOT invent fields.
                - DO NOT output code, explanations, Python, or any text outside JSON.
                - Do NOT rename fields.
                - Do NOT output anything except pure corrected JSON.
                - Maintain all data but correct data types only.
                -If a DateTime field appears as an object, extract the datetime value inside it and output only the ISO string.
                - For polymorphic properties (oneOf types):
                        • A type is considered VALID ONLY IF all of its REQUIRED properties are explicitly mentioned in the input text.
                        • If none of the oneOf schemas have all required fields present in the text:
                        → Set the polymorphic property value to null.
                - Ignore .NET list metadata fields like Capacity, Count, Item[] entirely.
                - ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.

                Return:
                {{ ""key"": ""value"" }} json format only with complex data types.
                ";
        }

        /// <summary>
        /// Gets rule file content from the specified path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string ReadRuleFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return string.Empty;
        }
    }
}