using System.Text.Json;
using System.Text.Json.Serialization;

namespace RagService.Api.Models;

public record RagQueryRequest(string Query, int TopK = 3);

public record RagQueryResponse(string Answer, List<RagSource> Sources);

public record RagSource(string Content, double Score, Guid DocumentId, JsonDocument? Metadata);

public record RagIngestRequest(string Content, JsonElement? Metadata = null);

public record RagIngestResponse(Guid Id, string Message);
