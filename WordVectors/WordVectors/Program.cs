using DocumentVectors;
using Microsoft.ML;

namespace DocumentVectors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start Extracting");

            string url1 = "https://en.wikipedia.org/wiki/ChatGPT";
            string url2 = "https://en.wikipedia.org/wiki/London";

            string code1path = @"C:/Users/mbasawatia/Downloads/Extract/__OUTPUT_FILE__";
            string code2path = @"C:/Users/mbasawatia/Downloads/Extract/Restore-Resourcedeleted1.json";

            string codeFile1 = @"C:/Users/mbasawatia/Downloads/Extract/GetAllCustomersResponseModel.cs";
            string codeFile2 = @"C:/Users/mbasawatia/Downloads/Extract/GetAllCustomersResponseModel2.cs";

            var similarityHtml = CompareHTMLPages(url1, url2);

            var similarityDocs = CompareDocuments(@"C:/Users/mbasawatia/Downloads/Extract/txt1.txt", @"C:/Users/mbasawatia/Downloads/Extract/txt2.txt");

            var similarityCode = CompareCodeFiles(code1path, code2path);

            var similarityCodeFiles = CompareCodeFiles(codeFile1, codeFile2);

            Console.WriteLine($"Similarity between HTML pages: {similarityHtml}");
            Console.WriteLine($"Similarity between Documents: {similarityDocs}");
            Console.WriteLine($"Similarity between Code files: {similarityCode}");
            Console.WriteLine($"Similarity between C# Code files: {similarityCodeFiles}");
        }

        public static double CompareDocuments(string path1, string path2)
        {
            var txt1 = ExtractFromDocument.ExtractTextDocument(@"C:/Users/mbasawatia/Downloads/Extract/txt1.txt");
            var txt2 = ExtractFromDocument.ExtractTextDocument(@"C:/Users/mbasawatia/Downloads/Extract/txt2.txt");


            var preprocessedDoc1 = PreprocessText.SanitizeText(txt1);
            var preprocessedDoc2 = PreprocessText.SanitizeText(txt2);

            List<string> documents = new List<string> { preprocessedDoc1, preprocessedDoc2 };
            var mlContext = new MLContext();
            var transformations = new Transformations(mlContext);
            var documentsTokens = transformations.CreateDocumentsTokens(documents);

            var doc1 = documentsTokens[0].Features;
            var doc2 = documentsTokens[1].Features;
            double sim = EmbeddingHelper.CosineSimilarity(doc1, doc2);

            return sim;
        }


        public static double CompareHTMLPages(string url1, string url2)
        {
            var txt1 = ExtractFromUrl.ExtractTextFromUrl(url1);
            var txt2 = ExtractFromUrl.ExtractTextFromUrl(url2);
            var preprocessedDoc1 = PreprocessText.SanitizeText(txt1);
            var preprocessedDoc2 = PreprocessText.SanitizeText(txt2);
            List<string> documents = new List<string> { preprocessedDoc1, preprocessedDoc2 };
            var mlContext = new MLContext();
            var transformations = new Transformations(mlContext);
            var documentsTokens = transformations.CreateDocumentsTokens(documents);
            var doc1 = documentsTokens[0].Features;
            var doc2 = documentsTokens[1].Features;
            double sim = EmbeddingHelper.CosineSimilarity(doc1, doc2);
            return sim;
        }

        public static double CompareCodeFiles(string filepath1, string filepath2)
        {
            var tokenizedCode1 = FileProcessor.ExtractAndTokenize(filepath1);
            var tokenizedCode2 = FileProcessor.ExtractAndTokenize(filepath2); 
            List<string> documents = new List<string> { tokenizedCode1, tokenizedCode2 };
            var mlContext = new MLContext();
            var transformations = new Transformations(mlContext);
            var documentsTokens = transformations.CreateDocumentsTokens(documents);
            var doc1 = documentsTokens[0].Features;
            var doc2 = documentsTokens[1].Features;
            double sim = EmbeddingHelper.CosineSimilarity(doc1, doc2);
            return sim;
        }
    }
}
