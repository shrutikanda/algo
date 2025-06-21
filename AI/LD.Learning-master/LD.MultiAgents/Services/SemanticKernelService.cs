using Azure.AI.OpenAI;
using LD.MultiAgents.AgentPlugins;
using LD.MultiAgents.Debug;
using LD.MultiAgents.Factories;
using LD.MultiAgents.Helper;
using LD.MultiAgents.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using System.Security.Cryptography;
using DebugLog = LD.MultiAgents.Debug.DebugLog;

namespace LD.MultiAgents.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly Kernel _kernel;
        private readonly ILoggerFactory loggerFactory;
        private List<Message> completionMessages = new List<Message>();
        List<LogProperty> _promptDebugProperties;
        ChatHistory chatHistory ;
        public SemanticKernelService(ILoggerFactory loggerFactory)
        {
             chatHistory = new ChatHistory();

            _kernel = Kernel.CreateBuilder()
           .AddAzureOpenAIChatCompletion(
               deploymentName: "LD-Chat", // Your Azure deployment name
               endpoint: "https://ld-llm-openai-poc.openai.azure.com/",
               apiKey: "c94f64b7a8e14874b94313a622463f4c"
           )
           .AddAzureOpenAITextEmbeddingGeneration(
               deploymentName: "text-embedding-3-large", // Your Azure deployment name for embeddings
               endpoint: "https://ld-llm-openai-poc.openai.azure.com/",
               apiKey: "c94f64b7a8e14874b94313a622463f4c")

          .Build();
            this.loggerFactory = loggerFactory;
        }
        public async Task<float[]> GenerateEmbedding(string text)
        {
            // Generate Embedding

            var embeddingModel = _kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();

            var embedding = await embeddingModel.GenerateEmbeddingAsync(text);

            // Convert ReadOnlyMemory<float> to IList<float>
            return embedding.ToArray();
        }
        private void LogMessage(string key, string value)
        {
            _promptDebugProperties.Add(new LogProperty(key, value));
        }

        public async Task<Tuple<List<Message>, List<Debug.DebugLog>>> GetResponseMultiAgent(Message userMessage, List<Message> messageHistory, string userId)
        {

            ChatFactory agentChatGeneratorService = new ChatFactory();

            var agentGroupChat = agentChatGeneratorService.BuildAgentGroupChat(_kernel, loggerFactory, LogMessage, userId);

            // Load history
            foreach (var chatMessage in messageHistory)
            {
                AuthorRole? role = AuthorRoleHelper.FromString(chatMessage.SenderRole);
                var chatMessageContent = new ChatMessageContent
                {
                    Role = role ?? AuthorRole.User,
                    Content = chatMessage.Text
                };
                agentGroupChat.AddChatMessage(chatMessageContent);
            }

            _promptDebugProperties = new List<LogProperty>();
            List<Message> completionMessages = new();
            List<DebugLog> completionMessagesLogs = new();
            do
            {
                var embeddings = await GenerateEmbedding(userMessage.Text);
                var userResponse = new ChatMessageContent(AuthorRole.User, userMessage.Text);
                agentGroupChat.AddChatMessage(userResponse);
                agentGroupChat.IsComplete = false;

                await foreach (ChatMessageContent response in agentGroupChat.InvokeAsync())
                {
                    string messageId = Guid.NewGuid().ToString();
                    string debugLogId = Guid.NewGuid().ToString();
                    completionMessages.Add(new Message(userMessage.UserId, userMessage.SessionId, response.AuthorName ?? string.Empty, 
                        response.Role.ToString(), response.Content ?? string.Empty, messageId, debugLogId));

                    if (_promptDebugProperties.Count > 0)
                    {
                        var completionMessagesLog = new DebugLog(userMessage.UserId, userMessage.SessionId, 
                            messageId, debugLogId);
                        completionMessagesLog.PropertyBag = _promptDebugProperties;
                        completionMessagesLogs.Add(completionMessagesLog);
                    }

                }
            }
            while (!agentGroupChat.IsComplete);
            return new Tuple<List<Message>, List<DebugLog>>(completionMessages, completionMessagesLogs);

        }

        public async Task<Tuple<List<Message>, List<Debug.DebugLog>>> GetResponse(Message userMessage, List<Message> messageHistory,  string userId)
        {
          
            ChatFactory agentChatGeneratorService = new ChatFactory();

            var agentGroupChat = agentChatGeneratorService.BuildAgent(_kernel, AgentType.ConversationAnalysis ,loggerFactory, userId);

            // Load history correctly
            foreach (var chatMessage in messageHistory)
            {
                AuthorRole role = AuthorRoleHelper.FromString(chatMessage.SenderRole) ?? AuthorRole.User;
                chatHistory.AddMessage(role, chatMessage.Text);
            }
            

            // Generate response
            List<Message> completionMessages = new List<Message>();

            await foreach (ChatMessageContent response in agentGroupChat.InvokeAsync(chatHistory))
            {
                string messageId = Guid.NewGuid().ToString();

                completionMessages.Add(new Message(
                    userMessage.UserId,
                    userMessage.SessionId,
                    response.AuthorName ?? string.Empty,
                    response.Role.ToString(),
                    response.Content,
                    messageId));
            }

            return new Tuple<List<Message>, List<Debug.DebugLog>>(
                completionMessages,
                new List<Debug.DebugLog>()); // You can optionally populate logs here
        
       

        //************************
        //var agent = new ChatCompletionAgent
        //{
        //    Kernel = _kernel.Clone(),
        //    Name = "BasicAgent",
        //    Description = "You are a helpful AI assistant.",
        //    Instructions = "Provide concise and informative answers."
        //};

        //ChatHistory chatHistory = new ChatHistory();

        //chatHistory.AddUserMessage(userMessage.Text);

        //await foreach (ChatMessageContent response in agent.InvokeAsync(chatHistory))
        //{
        //     string messageId = Guid.NewGuid().ToString();

        //    completionMessages.Add
        //        (new Message(userMessage.TenantId, userMessage.UserId, userMessage.SessionId,
        //        response.AuthorName ?? string.Empty, response.Role.ToString(),
        //        response.Content, messageId));

        //}

        //return new Tuple<List<Message>, List<DebugLog>>(
        //  completionMessages,
        // new  List<DebugLog>()
        //);

    }

        public Task ResetSemanticCache()
        {
            throw new NotImplementedException();
        }

        public Task<string> Summarize(string sessionId, string userPrompt)
        {
            throw new NotImplementedException();
        }
    }

    public interface ISemanticKernelService
    {
        Task<Tuple<List<Message>, List<DebugLog>>> GetResponse(Message userMessage, List<Message> messageHistory, string userId);

        Task<Tuple<List<Message>, List<DebugLog>>> GetResponseMultiAgent(Message userMessage, List<Message> messageHistory, string userId);


        Task<string> Summarize(string sessionId, string userPrompt);

        Task<float[]> GenerateEmbedding(string text);

        Task ResetSemanticCache();
    }


}
