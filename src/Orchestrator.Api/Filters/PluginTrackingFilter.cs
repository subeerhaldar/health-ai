using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Orchestrator.Api.Services;

namespace Orchestrator.Api.Filters;

public class PluginTrackingFilter : IFunctionInvocationFilter
{
    private readonly PluginTracker _tracker;

    public PluginTrackingFilter(PluginTracker tracker)
    {
        _tracker = tracker;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        _tracker.LastInvokedPlugin = context.Function.PluginName;
        await next(context);
    }
}
