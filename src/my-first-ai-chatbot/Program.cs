using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using OllamaSharp;

#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001

class Program
{
    static async Task Main()
    {
        // Connecting to Qdrant with the correct vector size (4096)
        var dbClient = new QdrantVectorDbClient("http://localhost:6333", vectorSize: 4096); // 4096 for Mistral
        var memoryStore = new QdrantMemoryStore(dbClient);

        // Setting up HttpClient for Ollama
        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };
        // Adding an embedding generator (Ollama)
        var embeddingGenerator = new OllamaApiClient(httpClient, "mistral").AsTextEmbeddingGenerationService();

        // Creating vector memory (in Qdrant)
        var memory = new SemanticTextMemory(memoryStore, embeddingGenerator);

        // Initializing the Kernel with a local AI model
        var kernel = Kernel.CreateBuilder()
            .AddOllamaChatCompletion("mistral", httpClient: httpClient)
            .Build();
        kernel.ImportPluginFromObject(new MemoryManagerPlugin(), "MemoryManager");

        Console.WriteLine("AI Chatbot (with improved context) – Type 'exit' to quit.");

        while (true)
        {
            Console.Write("Enter your question: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.ToLower() == "exit") break;

            // Searching for the closest matches in memory
            var searchResults = memory.SearchAsync("chat_history_multiturn", input, 3);

            // Combining several relevant responses
            string relatedMemory = (await searchResults.AnyAsync())
                ? string.Join("\n", (await searchResults.ToListAsync()).Select(r => r.Metadata.Text))
                : "No previous context available.";

            // Creating an AI function with enhanced context
            var function = kernel.CreateFunctionFromPrompt($@"
                Use the following information from the previous conversation:
                {relatedMemory}

                Respond clearly and concisely to the following question: {input}
            ");

            // Executing the AI function
            var result = await kernel.InvokeAsync(function);

            // Saving the conversation in Qdrant
            await memory.SaveInformationAsync("chat_history_multiturn", id: Guid.NewGuid().ToString(), text: $"{input} -> {result}");

            Console.WriteLine($"AI: {result}");
        }

        Console.WriteLine("The chatbot has been stopped.");
    }
}

public class MemoryManagerPlugin
{
    private readonly Dictionary<string, List<string>> conversationHistory = new();

    [KernelFunction]
    public void SaveToMemory(string user, string message)
    {
        if (!conversationHistory.ContainsKey(user))
            conversationHistory[user] = new List<string>();

        conversationHistory[user].Add(message);
    }

    [KernelFunction]
    public string RetrieveMemory(string user, int lastN = 5)
    {
        if (!conversationHistory.ContainsKey(user) || conversationHistory[user].Count == 0)
            return "No available memories.";

        var memory = conversationHistory[user].TakeLast(lastN);
        return string.Join("\n", memory);
    }

    [KernelFunction]
    public string SummarizeMemory(string user)
    {
        if (!conversationHistory.ContainsKey(user) || conversationHistory[user].Count == 0)
            return "No information to summarize.";

        var summary = conversationHistory[user].TakeLast(5);
        return $"Summary of the last conversations:\n{string.Join("\n", summary)}";
    }
}
