using System.Text.Json;
using Pgvector;

namespace RagService.Api.Entities;

public class DocumentChunk
{
    public Guid Id { get; set; }
    
    public required string Content { get; set; }
    
    public required Vector Embedding { get; set; }
    
    // Stored as JSONB in Postgres
    public JsonDocument? Metadata { get; set; }
}
