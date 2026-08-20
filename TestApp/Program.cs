using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using OpenAI;

class Program {
    static async Task Main() {
        var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "";
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://openrouter.ai/api/") };
        var openAIClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey), options);
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion("nvidia/nemotron-3.5-lightning:free", openAIClient);
        var kernel = kernelBuilder.Build();
        try {
            var result = await kernel.InvokePromptAsync("hi");
            Console.WriteLine("Success: " + result);
        } catch(Exception ex) {
            Console.WriteLine("Error:");
            Console.WriteLine(ex.ToString());
        }
    }
}
