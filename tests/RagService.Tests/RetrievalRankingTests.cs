using Moq;
using Pgvector;
using RagService.Api.Entities;
using RagService.Api.Services;
using Xunit;

namespace RagService.Tests;

public class RetrievalRankingTests
{
    [Fact]
    public async Task RankDocumentsAsync_ReturnsClosestDocumentsFirst()
    {
        // Arrange
        var mockEmbeddingService = new Mock<IEmbeddingService>();
        
        // Let's pretend our query embedding is [1, 0, 0]
        var queryEmbedding = new float[1024];
        queryEmbedding[0] = 1f; // [1, 0, 0, ...]
        mockEmbeddingService.Setup(x => x.GenerateEmbeddingAsync("Find medical history", It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryEmbedding);

        var searchService = new RagSearchService(mockEmbeddingService.Object);

        // Document A is [1, 0, 0] (Distance = 0, exact match)
        var vecA = new float[1024]; vecA[0] = 1f;
        var docA = new DocumentChunk { Id = Guid.NewGuid(), Content = "Doc A", Embedding = new Vector(vecA) };
        
        var vecB = new float[1024]; vecB[1] = 1f;
        var docB = new DocumentChunk { Id = Guid.NewGuid(), Content = "Doc B", Embedding = new Vector(vecB) };
        
        var vecC = new float[1024]; vecC[0] = 0.70710678f; vecC[1] = 0.70710678f;
        var docC = new DocumentChunk { Id = Guid.NewGuid(), Content = "Doc C", Embedding = new Vector(vecC) };

        var documents = new List<DocumentChunk> { docA, docB, docC };

        // Act
        var ranked = await searchService.RankDocumentsAsync("Find medical history", documents, topK: 2);

        // Assert
        Assert.Equal(2, ranked.Count);
        
        // 1st should be Doc A
        Assert.Equal(docA.Id, ranked[0].Document.Id);
        Assert.Equal(0.0, ranked[0].Distance, 5);

        // 2nd should be Doc C
        Assert.Equal(docC.Id, ranked[1].Document.Id);
        Assert.True(ranked[1].Distance > 0 && ranked[1].Distance < 1);
    }
}
