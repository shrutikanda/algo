using Azure;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddSingleton(new OpenAIClient(
    new Uri("https://ld-llm-openai-poc.openai.azure.com"),     
    new AzureKeyCredential("c94f64b7a8e14874b94313a622463f4c")
));
// register ChromaService with a shared HttpClient
builder.Services.AddHttpClient<IChromaService, ChromaService>();
builder.Services.AddSingleton<FunctionOrchestrator>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// In-memory conversation store
var conversations = new Dictionary<string, List<ChatMessage>>();

const string collectionNAME = "DCRM-knowledge-base";

app.MapPost("/chat", async (
    IChromaService chroma,
    OpenAIClient openAIClient,
    FunctionOrchestrator orchestrator,
    ChatRequest request) =>
{
    // Initialize conversation if not exists
    if (!conversations.ContainsKey(request.ConversationId))
    {
        conversations[request.ConversationId] = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant.")
        };
    }

    var chatHistory = conversations[request.ConversationId];
    chatHistory.Add(new ChatMessage(ChatRole.User, request.Message));

    // Trim history
    int maxMessages = 20;
    if (chatHistory.Count > maxMessages)
    {
        var systemPrompt = chatHistory.FirstOrDefault(m => m.Role == ChatRole.System);
        chatHistory = chatHistory.Skip(chatHistory.Count - maxMessages).ToList();
        if (systemPrompt != null) chatHistory.Insert(0, systemPrompt);
        conversations[request.ConversationId] = chatHistory;
    }

    // Embed query and retrieve context
    var embedRes = await openAIClient.GetEmbeddingsAsync(
        "text-embedding-3-large", new EmbeddingsOptions(request.Message));
    var userEmb = embedRes.Value.Data[0].Embedding.ToArray();

    var contexts = await chroma.QueryAsync(collectionNAME, userEmb, topK: 3);
    var ctxText = string.Join("\n\n---\n\n", contexts);
    chatHistory.Insert(1, new ChatMessage(ChatRole.System, "Here’s some relevant context:\n\n" + ctxText));

    // Build chat options
    var chatOptions = new ChatCompletionsOptions();
    chatOptions.Temperature = 0; // no hulicination 
    chatOptions.ChoiceCount = 1; // single response
    chatOptions.Functions.Add(FunctionCallingTools.GetLoanInformationTool());
    chatOptions.Functions.Add(FunctionCallingTools.GetCustomerServiceTool());
    chatOptions.Functions.Add(FunctionCallingTools.GetConversationSentimentTool());

    

    foreach (var msg in chatHistory)
        chatOptions.Messages.Add(msg);

    var response = await openAIClient.GetChatCompletionsAsync("LD-Chat", chatOptions);
    var choice = response.Value.Choices[0];

    string finalReply;

    if (choice.Message.FunctionCall is { } functionCall)
    {
        if (orchestrator.TryExecuteFunction(functionCall, out var functionResultTask))
        {
            var result = await functionResultTask;
            chatHistory.Add(new ChatMessage(ChatRole.Function, result) { Name = functionCall.Name });

            // Rebuild and re-query
            chatOptions = new ChatCompletionsOptions();
            foreach (var msg in chatHistory)
                chatOptions.Messages.Add(msg);

            var finalResponse = await openAIClient.GetChatCompletionsAsync("LD-Chat", chatOptions);
            finalReply = finalResponse.Value.Choices[0].Message.Content!;
            chatHistory.Add(new ChatMessage(ChatRole.Assistant, finalReply));
        }
        else
        {
            finalReply = "Sorry, I don’t know how to run that function.";
        }
    }
    else
    {
        finalReply = choice.Message.Content!;
        chatHistory.Add(new ChatMessage(ChatRole.Assistant, finalReply));
    }

    return Results.Ok(new { reply = finalReply });
});


app.MapPost("/embed", async (
     IChromaService chroma,
    OpenAIClient openAIClient,
    EmbeddingRequest embeddingRequest) =>
{
    try
    {
        var embeddingOptions = new EmbeddingsOptions(embeddingRequest.documentContent);
        var response = await openAIClient.GetEmbeddingsAsync("text-embedding-3-large", embeddingOptions);
        var embedding = response.Value.Data[0].Embedding;

        // Convert the embedding (which is a list of floats) into a 2D float array (float[][])
        var embeddingArray = embedding.ToArray();

        // 3) upsert it into Chroma
        await chroma.UpsertAsync(
            collectionId: collectionNAME,
            documentContent: embeddingRequest.documentContent,
            embedding: embeddingArray,            
            documentId: embeddingRequest.documentId
        );        

        return Results.Ok(new
        {
            embedding.Count,
            embedding = embedding.Take(10) // Just showing first 10 for brevity
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error generating embedding: {ex.Message}");
    }
});



app.Run();

record ChatRequest(string ConversationId, string Message);
record EmbeddingRequest(string documentContent,  string documentId);