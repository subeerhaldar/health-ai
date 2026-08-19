using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Orchestrator.Api.Plugins;

public class ClinicalGuidelinesPlugin
{
    private readonly HttpClient _httpClient;

    public ClinicalGuidelinesPlugin(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [KernelFunction, Description("Searches the hospital's medical literature and clinical guidelines for answers to medical questions.")]
    public async Task<string> SearchClinicalGuidelinesAsync([Description("The medical question or search query")] string query)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { Query = query }), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://ragservice-api:8080/rag/query", content);
        
        if (!response.IsSuccessStatusCode)
        {
            return $"Error searching guidelines: {response.StatusCode}";
        }
        return await response.Content.ReadAsStringAsync();
    }
}
