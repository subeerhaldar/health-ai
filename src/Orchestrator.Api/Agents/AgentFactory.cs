using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Orchestrator.Api.Plugins;

namespace Orchestrator.Api.Agents;

public class AgentFactory
{
    private readonly Kernel _baseKernel;

    public AgentFactory(Kernel baseKernel)
    {
        _baseKernel = baseKernel;
    }

    public ChatCompletionAgent CreatePatientRecordsAgent()
    {
        var kernel = _baseKernel.Clone();
        kernel.Plugins.Clear();
        kernel.Plugins.AddFromType<PatientDataPlugin>();

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
        kernel.Plugins.AddFromType<ClinicalGuidelinesPlugin>();

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
        kernel.Plugins.AddFromType<WorkflowPlugin>();

        return new ChatCompletionAgent
        {
            Name = "WorkflowExecutionAgent",
            Instructions = "Specialist AI Agent in initiating hospital workflows, appointment scheduling, prescription refills, and saga automation.",
            Kernel = kernel
        };
    }
}
