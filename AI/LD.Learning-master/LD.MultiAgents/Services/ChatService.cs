using Azure.Core;
using LD.MultiAgents.AgentPlugins;
using LD.MultiAgents.Models;
using Microsoft.Extensions.AI;
using System.Collections.Generic;

namespace LD.MultiAgents.Services
{
    public interface IChatService
    {
        Task<List<Message>> GetChatCompletionAsync(string userId, string? sessionId, string userPrompt);
    }

    public class ChatService : IChatService
    {
        private readonly ISemanticKernelService semanticKernelService;

        private readonly Dictionary<string, List<Message>> archivedMessages;

        public ChatService(ISemanticKernelService semanticKernelService)
        {
            this.semanticKernelService = semanticKernelService;
            archivedMessages = new Dictionary<string, List<Message>>();
        }

        public async Task<List<Message>> GetChatCompletionAsync(string userId, string sessionId, string userPrompt)
        {
            var userMessage = new Message(userId, sessionId, "User", "User", userPrompt);

            // Get or create user's chat history
            if (!archivedMessages.TryGetValue(userId, out var chatHistory))
            {
                chatHistory = new List<Message>();
                archivedMessages[userId] = chatHistory;
            }

            // Add the new message to the history
            chatHistory.Add(userMessage);

            var result = await semanticKernelService.GetResponseMultiAgent(userMessage, chatHistory, userId);

            return result.Item1;
        }
    }
}
