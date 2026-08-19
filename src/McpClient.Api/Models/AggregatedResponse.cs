namespace McpClient.Api.Models;

public class AggregatedResponse<T>
{
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsDegraded => Errors.Any();

    public AggregatedResponse(T? data)
    {
        Data = data;
    }
}
