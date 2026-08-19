using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using OpenAI;

class Program {
    static async Task Main() {
        var apiKey = "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";
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://openrouter.ai/api/") };
        var openAIClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey), options);
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion("google/gemma-2-9b-it:free", openAIClient);
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
