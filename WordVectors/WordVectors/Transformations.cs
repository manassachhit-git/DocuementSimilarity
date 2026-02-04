using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentVectors
{
    public class Transformations
    {
        public MLContext MLContext { get; set; }
        public Transformations(MLContext mLContext) => MLContext = mLContext;
        public List<TransformedText> CreateTokens(string preprocessedText)
        {
            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromEnumerable(new List<InputText>
            {
                new InputText { Text = preprocessedText }
            });

            var pipeline = GenerateWordVectors.CreateWordVectors(mlContext);

            var model = pipeline.Fit(data);
            var transformed = model.Transform(data);

            // Map transformed output into our POCO
            var tokensWithVectors = mlContext.Data.CreateEnumerable<TransformedText>(transformed, reuseRowObject: false).ToList();

            return tokensWithVectors;
        }

        public List<TransformedText> CreateDocumentsTokens(List<string> documents)
        {
            var mlContext = new MLContext();

            // Load all documents together
            var data = mlContext.Data.LoadFromEnumerable(documents.Select(d => new InputText { Text = d }));

            // One pipeline for all docs
            var pipeline = mlContext.Transforms.Text.NormalizeText("Normalized", "Text")
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Normalized"))
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Tokens"));

            var model = pipeline.Fit(data);
            var transformed = model.Transform(data);

            // Extract feature vectors
            var tokens = mlContext.Data.CreateEnumerable<TransformedText>(transformed, reuseRowObject: false).ToList();
            return tokens;
        }

    }


    public class InputText
    {
        public string? Text { get; set; }
    }

    public class TransformedText
    {
        [ColumnName("Tokens")]
        public string[]? Tokens { get; set; }

        [ColumnName("Features")]
        public float[] Features { get; set; } // embedding vector
    }
}
