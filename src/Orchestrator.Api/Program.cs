using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.SemanticKernel;
using Orchestrator.Api.Plugins;
using Orchestrator.Api.Services;
using Orchestrator.Api.Filters;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

// Track invoked plugins
builder.Services.AddScoped<PluginTracker>();
builder.Services.AddTransient<IFunctionInvocationFilter, PluginTrackingFilter>();

// HTTP Clients for Plugins
builder.Services.AddHttpClient<PatientDataPlugin>();
builder.Services.AddHttpClient<ClinicalGuidelinesPlugin>();
builder.Services.AddHttpClient<WorkflowPlugin>();

// Semantic Kernel setup
var openRouterConfig = builder.Configuration.GetSection("OpenRouter");
var apiKey = Environment.GetEnvironmentVariable(openRouterConfig["ApiKeyEnvVar"] ?? "OPENROUTER_API_KEY") ?? "missing_key";

var kernelBuilder = builder.Services.AddKernel();

var options = new OpenAI.OpenAIClientOptions
{
    Endpoint = new Uri(openRouterConfig["BaseUrl"] ?? "https://openrouter.ai/api/")
};
var openAIClient = new OpenAI.OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey), options);

kernelBuilder.AddOpenAIChatCompletion(
    modelId: openRouterConfig["CompletionModel"] ?? "google/gemma-2-9b-it:free",
    openAIClient: openAIClient
);

kernelBuilder.Plugins.AddFromType<PatientDataPlugin>();
kernelBuilder.Plugins.AddFromType<ClinicalGuidelinesPlugin>();
kernelBuilder.Plugins.AddFromType<WorkflowPlugin>();

// CORS to allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapHub<Orchestrator.Api.Hubs.ChatHub>("/orchestrator/chatHub");

app.MapGet("/", () => "Orchestrator API is running!");

app.Run();

public record ChatRequest(string PatientId, string Message);
