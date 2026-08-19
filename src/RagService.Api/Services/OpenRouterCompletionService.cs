using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RagService.Api.Services;

public class OpenRouterCompletionService : ICompletionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenRouterCompletionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateAnswerAsync(string query, IEnumerable<string> context, CancellationToken cancellationToken = default)
    {
        var model = _configuration["OpenRouter:CompletionModel"];
        if (string.IsNullOrEmpty(model))
        {
            model = "google/gemma-2-9b-it:free";
        }
        
        var envVar = _configuration["OpenRouter:ApiKeyEnvVar"] ?? "OPENROUTER_API_KEY";
        var apiKey = Environment.GetEnvironmentVariable(envVar) ?? "dummy_key";

        var contextText = string.Join("\n\n---\n\n", context);
        var systemPrompt = $"You are a helpful healthcare AI. Answer the user's query based ONLY on the following context. If the answer is not in the context, say so.\n\nContext:\n{contextText}";

        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = query }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        using var jsonDocument = JsonDocument.Parse(responseContent);

        var answer = jsonDocument.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return answer ?? string.Empty;
    }
}
