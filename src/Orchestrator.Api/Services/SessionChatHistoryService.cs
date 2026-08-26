using System.Collections.Concurrent;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Orchestrator.Api.Services;

public class SessionChatHistoryService
{
    private readonly ConcurrentDictionary<string, ChatHistory> _sessions = new();

    public ChatHistory GetOrCreateHistory(string sessionId, string role, string patientId)
    {
        return _sessions.GetOrAdd(sessionId, _ =>
        {
            var history = new ChatHistory();
            history.AddSystemMessage($@"You are a highly capable AI Healthcare Assistant (Copilot).
The user is a {role} and they are asking about Patient ID: {patientId}.
Answer their question accurately using the available specialized tools and agents.
Maintain full conversational context across all turns, including dates, times, medication names, and prior requested actions.");
            return history;
        });
    }

    public void ClearSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }
}
