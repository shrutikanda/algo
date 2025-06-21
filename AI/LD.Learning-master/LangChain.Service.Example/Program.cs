﻿using LangChain.Memory;
using LangChain.Providers.Azure;
using LangChain.Serve;
using LangChain.Serve.Abstractions.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ollama;
using static LangChain.Chains.Chain;
using Message = LangChain.Providers.Message;
using MessageRole = LangChain.Providers.MessageRole;

var builder = WebApplication.CreateBuilder();

// 1. Add LangChainServe
builder.Services.AddLangChainServe();

// 2. Create a model
var provider = new AzureOpenAiProvider(apiKey: "3995e126ae09492187fc672efaa3e20a",
                  endpoint: "https://ldapi-sg1.loandepotdev.works/LLM-POC/",
                 deploymentID: "LD-Chat");
var model = new AzureOpenAiChatModel(provider, id: "LD-Chat");

// 3. Optional. Add custom name generator
builder.Services.AddCustomNameGenerator(async messages =>
{
    var template =
        @"You will be given conversation between User and Assistant. Your task is to give name to this conversation using maximum 3 words
Conversation:
{chat_history}
Your name: ";
    var conversationBufferMemory = await ConvertToConversationBuffer(messages);
    var chain = LoadMemory(conversationBufferMemory, "chat_history")
                | Template(template)
                | LLM(model);

    return await chain.RunAsync("text") ?? string.Empty;
});

// 4. Optional. Add swagger to be able to test the API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware to capture response
app.Use(async (context, next) =>
{
    var originalBodyStream = context.Response.Body;

    using (var responseBody = new MemoryStream())
    {
        context.Response.Body = responseBody;

        await next();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Log or capture the responseText here
        Console.WriteLine(responseText);

        await responseBody.CopyToAsync(originalBodyStream);
    }
});

// 5. Configure LangChainServe
app.UseLangChainServe(options =>
{
    options.RegisterModel("Test model", async (messages) =>
    {
        var template = @"You are helpful assistant. Keep your answers short.
{chat_history}
Assistant:";
        var conversationBufferMemory = await ConvertToConversationBuffer(messages);
        var chain = LoadMemory(conversationBufferMemory, "chat_history")
                    | Template(template)
                    | LLM(model);

        var response = await chain.RunAsync("text");
        return new StoredMessage
        {
            Author = MessageAuthor.Ai,
            Content = response ?? string.Empty,
        };
    });

    options.RegisterModel("LoanDepot model", async (messages) =>
    {
        var template = @"You are helpful Loandepot LoanOfficer. Keep your answers short.
{chat_history}
Assistant:";
        var conversationBufferMemory = await ConvertToConversationBuffer(messages);
        var chain = LoadMemory(conversationBufferMemory, "chat_history")
                    | Template(template)
                    | LLM(model);

        var response = await chain.RunAsync("text");
        return new StoredMessage
        {
            Author = MessageAuthor.Ai,
            Content = response ?? string.Empty,
        };
    });
});

// 6. Optional. Add swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.Run();
return;

async Task<ConversationBufferMemory> ConvertToConversationBuffer(IReadOnlyCollection<StoredMessage> list)
{
    var conversationBufferMemory = new ConversationBufferMemory
    {
        Formatter =
        {
            HumanPrefix = "User",
            AiPrefix = "Assistant",
        }
    };
    List<Message> converted = list
        .Select(x => new Message(x.Content, x.Author == MessageAuthor.User ? MessageRole.Human : MessageRole.Ai))
        .ToList();

    await conversationBufferMemory.ChatHistory.AddMessages(converted);

    return conversationBufferMemory;
}