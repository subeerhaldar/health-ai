using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Orchestrator.Api.Services;

namespace Orchestrator.Api.Hubs;

public class ChatHub : Hub
{
    private readonly Kernel _kernel;
    private readonly PluginTracker _tracker;

    public ChatHub(Kernel kernel, PluginTracker tracker)
    {
        _kernel = kernel;
        _tracker = tracker;
    }

    public async Task SendMessage(string userId, string role, string message)
    {
        _tracker.LastInvokedPlugin = null;

        try
        {
            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var prompt = $@"
You are a highly capable AI Healthcare assistant (Copilot).
The user is a {role} and they are asking about Patient ID: {userId}.
Answer their question using the tools available to you.

User: {message}
";

            var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments(settings));
            
            string sourceService = _tracker.LastInvokedPlugin switch
            {
                "PatientDataPlugin" => "MCP",
                "ClinicalGuidelinesPlugin" => "RAG",
                "WorkflowPlugin" => "Workflow",
                _ => "Orchestrator AI"
            };

            await Clients.Caller.SendAsync("ReceiveMessage", result.GetValue<string>(), sourceService);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", $"Error: {ex.Message}", "Error");
        }
    }
}
