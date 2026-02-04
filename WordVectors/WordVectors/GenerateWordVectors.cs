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
    public static class GenerateWordVectors
    {
        public static EstimatorChain<ITransformer> CreateWordVectors(MLContext mlContext)
        {
            var pipeline = mlContext.Transforms.Text.NormalizeText("Normalized", "Text")
            .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Normalized"))
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Tokens"));

            return pipeline;
        }
    }
}
