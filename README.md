# DocumentVectors

A .NET 8 library for document similarity analysis using machine learning-based vector embeddings. This project enables intelligent document comparison by converting documents into numerical vector representations and measuring their semantic similarity.

## Overview

DocumentVectors processes various document formats (including Word documents and HTML) and uses ML.NET to generate vector embeddings that capture the semantic meaning of text content. This approach enables accurate document similarity comparisons beyond simple keyword matching.

## Features

- **Multi-format Document Support**: Parse and process documents in multiple formats:
  - Microsoft Word documents (`.docx`) via DocumentFormat.OpenXml
  - HTML documents via HtmlAgilityPack
- **ML-Powered Vector Embeddings**: Leverage Microsoft.ML for generating semantic vector representations
- **Document Similarity Analysis**: Compare documents based on their vector representations
- **Extensible Architecture**: Easily add support for additional document formats

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or later (recommended) or any compatible IDE

## Installation

Clone the repository: git clone https://github.com/manassachhit-git/DocuementSimilarity.git cd DocuementSimilarity

Restore dependencies:

Build the project: dotnet build

## Dependencies

- **DocumentFormat.OpenXml** (v3.3.0): Microsoft Word document processing
- **HtmlAgilityPack** (v1.12.2): HTML parsing and manipulation
- **Microsoft.ML** (v4.0.2): Machine learning and vector embedding generation
- **xUnit** (v2.9.3): Unit testing framework

## Usage
// Example usage (adjust based on your actual implementation) using DocumentVectors;
// Load documents var doc1 = DocumentLoader.Load("path/to/document1.docx"); var doc2 = DocumentLoader.Load("path/to/document2.docx");
// Generate vector embeddings var vector1 = VectorGenerator.GenerateEmbedding(doc1); var vector2 = VectorGenerator.GenerateEmbedding(doc2);
// Calculate similarity score var similarityScore = SimilarityCalculator.Compare(vector1, vector2); Console.WriteLine($"Similarity Score: {similarityScore}");

## Running Tests

Execute the test suite using:

## Use Cases

- **Document Deduplication**: Identify and remove duplicate or near-duplicate documents
- **Content Recommendation**: Suggest related documents based on semantic similarity
- **Plagiarism Detection**: Compare documents to detect potential content reuse
- **Document Clustering**: Group similar documents for organization and analysis

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
- Document processing powered by [DocumentFormat.OpenXml](https://github.com/OfficeDev/Open-XML-SDK)
- HTML parsing by [HtmlAgilityPack](https://html-agility-pack.net/)

## Contact

Manas Sachhit - [@manassachhit-git](https://github.com/manassachhit-git)
Project Link: [https://github.com/manassachhit-git/DocuementSimilarity](https://github.com/manassachhit-git/DocuementSimilarity)
