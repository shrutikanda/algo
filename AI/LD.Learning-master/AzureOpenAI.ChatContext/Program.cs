using Azure.AI.OpenAI;
using Azure;

// In-memory store for multiple conversations
Dictionary<string, List<ChatMessage>> conversationStore = new();

var endpoint = new Uri("https://ld-llm-openai-poc.openai.azure.com");
var apiKey = "c94f64b7a8e14874b94313a622463f4c";
var credentials = new AzureKeyCredential(apiKey);

var client = new OpenAIClient(endpoint, credentials);
Console.WriteLine("Welcome! Type a conversation ID (or leave empty to create new): ");
string conversationId = Console.ReadLine()?.Trim();

if (string.IsNullOrEmpty(conversationId))
{
    conversationId = Guid.NewGuid().ToString();
    Console.WriteLine($"🆕 New conversation started. Your ID: {conversationId}");
}
else if (!conversationStore.ContainsKey(conversationId))
{
    Console.WriteLine("⚠️ No conversation found with that ID. Starting a new one.");
}

if (!conversationStore.ContainsKey(conversationId))
{
    conversationStore[conversationId] = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant.")
            };
}

var chatHistory = conversationStore[conversationId];

while (true)
{
    Console.Write("User: ");
    string userInput = Console.ReadLine();

    if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

    var chatOptions = new ChatCompletionsOptions()
    {
        Temperature = 0.7f,
        MaxTokens = 1000,
        NucleusSamplingFactor = 0.95f
    };

    // Add history to the request
    foreach (var message in chatHistory)
        chatOptions.Messages.Add(message);

    var response = await client.GetChatCompletionsAsync("LD-Chat", chatOptions);
    var assistantReply = response.Value.Choices[0].Message.Content;

    Console.WriteLine($"Assistant: {assistantReply}");

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, assistantReply));
}
    
