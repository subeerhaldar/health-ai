using Pgvector;
using RagService.Api.Entities;

namespace RagService.Api.Services;

public class RagSearchService
{
    private readonly IEmbeddingService _embeddingService;

    public RagSearchService(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    /// <summary>
    /// Performs an in-memory cosine distance ranking of the provided documents against the query.
    /// In a real production scenario with millions of rows, you would push this into the database
    /// using EF Core's OrderBy(x => x.Embedding.CosineDistance(queryEmbedding)).
    /// This method is extracted to allow unit testing of the ranking logic with an in-memory fake of IEmbeddingService.
    /// </summary>
    public async Task<List<(DocumentChunk Document, double Distance)>> RankDocumentsAsync(
        string query, 
        IEnumerable<DocumentChunk> documents, 
        int topK)
    {
        var queryEmbeddingArray = await _embeddingService.GenerateEmbeddingAsync(query);
        var queryEmbedding = new Vector(queryEmbeddingArray);

        var ranked = documents
            .Select(doc => new { Document = doc, Distance = CosineDistance(doc.Embedding.ToArray(), queryEmbeddingArray) })
            .OrderBy(x => x.Distance)
            .Take(topK)
            .Select(x => (x.Document, x.Distance))
            .ToList();

        return ranked;
    }

    private double CosineDistance(float[] v1, float[] v2)
    {
        if (v1.Length != v2.Length) throw new ArgumentException("Vectors must have the same length");

        double dotProduct = 0.0;
        double norm1 = 0.0;
        double norm2 = 0.0;

        for (int i = 0; i < v1.Length; i++)
        {
            dotProduct += v1[i] * v2[i];
            norm1 += v1[i] * v1[i];
            norm2 += v2[i] * v2[i];
        }

        if (norm1 == 0 || norm2 == 0) return 1.0; // Max distance if one vector is 0
        
        var similarity = dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
        return 1.0 - similarity; // distance = 1 - similarity
    }
}
