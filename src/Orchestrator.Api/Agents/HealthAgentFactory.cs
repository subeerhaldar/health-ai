using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Orchestrator.Api.Plugins;

namespace Orchestrator.Api.Agents;

public class HealthAgentFactory
{
    private readonly Kernel _baseKernel;
    private readonly PatientDataPlugin _patientDataPlugin;
    private readonly ClinicalGuidelinesPlugin _clinicalGuidelinesPlugin;
    private readonly WorkflowPlugin _workflowPlugin;

    public HealthAgentFactory(
        Kernel baseKernel,
        PatientDataPlugin patientDataPlugin,
        ClinicalGuidelinesPlugin clinicalGuidelinesPlugin,
        WorkflowPlugin workflowPlugin)
    {
        _baseKernel = baseKernel;
        _patientDataPlugin = patientDataPlugin;
        _clinicalGuidelinesPlugin = clinicalGuidelinesPlugin;
        _workflowPlugin = workflowPlugin;
    }

    public ChatCompletionAgent CreatePatientRecordsAgent()
    {
        var kernel = _baseKernel.Clone();
        kernel.Plugins.Clear();
        kernel.Plugins.AddFromObject(_patientDataPlugin);

        return new ChatCompletionAgent
        {
            Name = "PatientRecordsAgent",
            Instructions = "Specialist AI Agent in retrieving EMR, patient lab results, medical history, and demographics.",
            Kernel = kernel
        };
    }

    public ChatCompletionAgent CreateClinicalGuidelinesAgent()
    {
        var kernel = _baseKernel.Clone();
        kernel.Plugins.Clear();
        kernel.Plugins.AddFromObject(_clinicalGuidelinesPlugin);

        return new ChatCompletionAgent
        {
            Name = "ClinicalGuidelinesAgent",
            Instructions = "Specialist AI Agent in querying medical literature, RAG clinical guidelines, hypertension/diabetes protocols, and evidence-based clinical recommendations.",
            Kernel = kernel
        };
    }

    public ChatCompletionAgent CreateWorkflowExecutionAgent()
    {
        var kernel = _baseKernel.Clone();
        kernel.Plugins.Clear();
        kernel.Plugins.AddFromObject(_workflowPlugin);

        return new ChatCompletionAgent
        {
            Name = "WorkflowExecutionAgent",
            Instructions = @"Specialist AI Agent in initiating hospital workflows, appointment scheduling, prescription refills, and saga automation.
CRITICAL INSTRUCTION FOR APPOINTMENTS:
Always inspect the ENTIRE multi-turn conversation history.
If the user specified a time (e.g., '9:00 AM', '14:30', '3 PM') in any prior message or the current message, and a date (e.g., 'next Monday', '2026-08-31') in any message, COMBINE THEM into a complete ISO 8601 date-time string (e.g., '2026-08-31T09:00:00Z').
Never drop the time component if it was mentioned in the conversation history.",
            Kernel = kernel,
            Arguments = new KernelArguments(new Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions
            })
        };
    }
}
