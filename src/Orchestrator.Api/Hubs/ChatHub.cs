using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Orchestrator.Api.Agents;
using Orchestrator.Api.Services;

namespace Orchestrator.Api.Hubs;

public class ChatHub : Hub
{
    private readonly HealthAgentFactory _agentFactory;
    private readonly SessionChatHistoryService _historyService;
    private readonly PluginTracker _tracker;

    public ChatHub(HealthAgentFactory agentFactory, SessionChatHistoryService historyService, PluginTracker tracker)
    {
        _agentFactory = agentFactory;
        _historyService = historyService;
        _tracker = tracker;
    }

    public async Task SendMessage(string userId, string role, string message)
    {
        _tracker.LastInvokedPlugin = null;

        try
        {
            var sessionId = string.IsNullOrWhiteSpace(userId) ? Context.ConnectionId : userId;
            var history = _historyService.GetOrCreateHistory(sessionId, role, userId);

            // Add user message to multi-turn session history
            history.AddUserMessage(message);

            // Select specialized agent based on prompt intent
            ChatCompletionAgent agent;
            var lowerMsg = message.ToLowerInvariant();
            if (lowerMsg.Contains("schedule") || lowerMsg.Contains("appointment") || lowerMsg.Contains("refill") || lowerMsg.Contains("workflow") || lowerMsg.Contains("monday") || lowerMsg.Contains("tuesday") || lowerMsg.Contains("wednesday") || lowerMsg.Contains("thursday") || lowerMsg.Contains("friday") || lowerMsg.Contains("saturday") || lowerMsg.Contains("sunday") || lowerMsg.Contains("am") || lowerMsg.Contains("pm") || lowerMsg.Contains("clock"))
            {
                agent = _agentFactory.CreateWorkflowExecutionAgent();
            }
            else if (lowerMsg.Contains("guideline") || lowerMsg.Contains("protocol") || lowerMsg.Contains("treatment") || lowerMsg.Contains("literature") || lowerMsg.Contains("hypertension") || lowerMsg.Contains("diabetes"))
            {
                agent = _agentFactory.CreateClinicalGuidelinesAgent();
            }
            else
            {
                agent = _agentFactory.CreatePatientRecordsAgent();
            }

            // Execute agent against session chat history
            var responseBuilder = new StringBuilder();
            await foreach (var responseItem in agent.InvokeAsync(history))
            {
                if (responseItem.Message != null && !string.IsNullOrEmpty(responseItem.Message.Content))
                {
                    responseBuilder.Append(responseItem.Message.Content);
                    history.Add(responseItem.Message);
                }
            }

            string finalAnswer = responseBuilder.ToString();
            if (string.IsNullOrWhiteSpace(finalAnswer))
            {
                finalAnswer = "Processed your request successfully.";
            }

            string sourceService = _tracker.LastInvokedPlugin switch
            {
                "PatientDataPlugin" => "MCP",
                "ClinicalGuidelinesPlugin" => "RAG",
                "WorkflowPlugin" => "Workflow",
                _ => agent.Name ?? "Health AI Agent"
            };

            await Clients.Caller.SendAsync("ReceiveMessage", finalAnswer, sourceService);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", $"Error: {ex.Message}", "Error");
        }
    }

    public void ClearSessionHistory(string userId)
    {
        var sessionId = string.IsNullOrWhiteSpace(userId) ? Context.ConnectionId : userId;
        _historyService.ClearSession(sessionId);
    }
}
