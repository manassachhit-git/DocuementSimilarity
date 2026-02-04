using EnglishToClassDefinition.SchemaBuilder;
using EnglishToClassDefinition.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Utilities
{
    internal class EnglishToJsonConverterUtility
    {
        private readonly LamaAIConverterService _ai;
        private readonly SelfCorrectorLLamaService _s_ai;

        public EnglishToJsonConverterUtility(LamaAIConverterService ai)
        {
            _ai = ai;
        }

        public EnglishToJsonConverterUtility(SelfCorrectorLLamaService ai)
        {
            _s_ai = ai;
        }
        public async Task<string> ConvertAsyncWithSelfCorrection<T>(string text)
        {
            var schema = ClassSchemaBuilder.Build(typeof(T));
            var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var props = Utility.GetPropertyNames(typeof(T));

            string prompt = $@"
            You are a structured data extraction engine. Your job is to extract structured JSON from English text, strictly following the provided schema and class definition.

            Rules:
            - Do NOT guess, infer, or add properties not present in the schema.
            - If a field is missing or invalid, omit it from ExtractedData and report the issue in ValidationErrors.
            - For arrays/lists, output as JSON arrays only.
            - For enums, output only the enum string value.
            - For polymorphic (oneOf) properties, only include a type if all its required fields are present; otherwise, set to null.
            - Ignore .NET list metadata fields like Capacity, Count, Item[].
            - Do NOT output explanations, comments, markdown, or any text outside the required JSON objects.
            - Maintain exact data types from the schema.
            - Do NOT add default or placeholder values unless specified by the schema.
            - Ignore .NET list metadata fields like Capacity, Count, Item[] entirely.
            - ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.

            Input Text:
            {text}

            Required JSON keys: {string.Join(", ", props)}

            Output:
            -ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.
            {{{{ """"key"""": """"value"""" }}}} json format only with complex data types. 
            "";
            ";

            var extractPayload = new
            {
                model = "meta/llama-4-maverick-17b-128e-instruct-maas",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                naturalText = text,
            };


            string firstPass = await _s_ai.SendAsync(extractPayload);


            // Fetch strict rules from file
            string strictRules = "";
            string ruleFilePath = @"C:\Users\mbasawatia\source\repos\WordVectors\EnglishToClassDefination\RuleFiles\Roledetails_Rule.txt";
            strictRules = ReadRuleFile(ruleFilePath);

            string prompt2 = $@"
            You are a strict JSON type corrector.

            Given:
            1. The target C# schema:
            {schemaJson}

            2. The JSON objects that needs correction:
            {firstPass}

            3. Strict Rules:
            {strictRules}
            
            Output Requirements:
            1. ExtractedData: Output a JSON object that strictly adheres to the provided C# schema.
               - All required fields must be present.
               - Data types must match exactly (e.g., strings, numbers, arrays, objects).
               - For array fields, ensure the output is a JSON array.
               - For enum fields, output only the enum string value.
               - For polymorphic (oneOf) properties, include a type only if all its required fields are present; otherwise, set to null.
               - Ignore .NET metadata fields entirely.
            4. ValidationErrors: Output a JSON array of human-friendly validation errors .NET metadata fields such as Capacity, Count, Comparer, Keys, Values, or Item. Ignore these fields entirely.
               - Each error must specify the field name and the reason for failure (e.g., missing, invalid type, out of range, pattern mismatch).
               - If there are no errors, return an empty array [].
               - if the field is not present that doesn't meant its required ignored those validation.
               - Output ONLY a pure JSON object — no explanation or comments.
               - ABSOLUTELY NO markdown formatting such as ```json, ``` or backticks.

            Validation Rules:
            - Validate all fields against schema constraints, including:
              • Required fields
              • Range (minimum/maximum)
              • Regular expression patterns
              • Any custom error messages from the schema

            High Priority Ignoring Rule:
            Never inspect, validate, or mention .NET metadata fields including:
            Count, Capacity, Comparer, Keys, Values, Item, or any sub-fields of these.

            These fields must NOT:
            - appear in ExtractedData
            - appear in ValidationErrors
            - be considered missing or invalid

            Task:
            - Do not use old Data.
            - Fix all mismatches to match the C# schema exactly.
            - If the schema expects an array (e.g. string[]):
                  - If the JSON value contains a comma-separated string, split by \"",\"" into an array with individual values wrapped in Quotes \""\"".
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
            var correctionPayload = new
            {
                model = "meta/llama-4-maverick-17b-128e-instruct-maas",
                messages = new[]
                {
                    new { role = "user", content = prompt2}

                },
                schema,
                modelOutput = JsonSerializer.Deserialize<object>(firstPass)
            };


            string cleaned = await _s_ai.SendAsync(correctionPayload);
            return Utility.CleanJsonFences(cleaned);
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
