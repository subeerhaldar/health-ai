namespace RagService.Api.Services;

public interface ICompletionService
{
    /// <summary>
    /// Generates an answer to the query, grounded in the provided context.
    /// </summary>
    Task<string> GenerateAnswerAsync(string query, IEnumerable<string> context, CancellationToken cancellationToken = default);
}
