using LD.MultiAgents.Services;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.MultiAgent
{
    public class ChatEndpoints
    {
        private readonly IChatService _chatService;
        const string collectionNAME = "DCRM-knowledge-base";
        public ChatEndpoints(IChatService chatService)
        {
            _chatService = chatService;
        }

        public void Map(WebApplication app)
        {
            app.MapPost("/tenant/{tenantId}/user/{userId}/sessions/{sessionId}/completion", async (string tenantId, string userId, string sessionId, [FromBody] string userPrompt) =>
             await _chatService.GetChatCompletionAsync( userId, sessionId, userPrompt))
            .WithName("GetChatCompletionDebugLogAsync");

            app.MapPost("/embed", async (
                                IChromaService chroma,
                                ISemanticKernelService semanticKernelService,
                                 EmbeddingRequest embeddingRequest) =>
            {
                try
                {
                  var embeddingArray =  await semanticKernelService.GenerateEmbedding(embeddingRequest.documentContent);

                    // 3) upsert it into Chroma
                    await chroma.UpsertAsync(
                        collectionId: collectionNAME,
                        documentContent: embeddingRequest.documentContent,
                        embedding: embeddingArray,
                        documentId: embeddingRequest.documentId
                    );

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error generating embedding: {ex.Message}");
                }
            });

        }

    }
    record EmbeddingRequest(string documentContent, string documentId);
}
