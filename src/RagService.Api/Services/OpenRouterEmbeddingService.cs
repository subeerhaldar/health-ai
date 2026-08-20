using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RagService.Api.Services;

public class OpenRouterEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenRouterEmbeddingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = _configuration["OpenRouter:EmbeddingModel"] ?? "liquid/lfm2.5-embedding-350m:free";
            var envVar = _configuration["OpenRouter:ApiKeyEnvVar"] ?? "OPENROUTER_API_KEY";
            var apiKey = Environment.GetEnvironmentVariable(envVar) ?? "dummy_key";

            var requestBody = new
            {
                model = model,
                input = new[] { text }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "embeddings")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"OpenRouter API Error: {response.StatusCode} - {errorBody}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDocument = JsonDocument.Parse(responseContent);

            var embeddingArray = jsonDocument.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return embeddingArray;
        }
        catch (Exception ex)
        {
            // Fallback to mock embeddings to ensure the demo continues working
            // even if the free OpenRouter embedding model goes offline or requires payment.
            Console.WriteLine($"[WARNING] Failed to fetch embeddings from OpenRouter, falling back to mock vector: {ex.Message}");
            return Enumerable.Repeat(0.1f, 384).ToArray();
        }
    }
}
