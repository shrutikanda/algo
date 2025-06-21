using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LD.MultiAgents.AgentPlugins
{
    public class ConversationAnalysisPlugin : BasePlugin
    {

        private readonly HttpClient client = new HttpClient();

        private string ENTERPRISE_CONVERSATIONSERVICE_BASEURL = "https://enterprise-conversationservice.dv1.loandepotdev.io/api/v1";

        public ConversationAnalysisPlugin(ILogger<BasePlugin> logger, string userId)
     : base(logger, userId)
        {
        }

        [KernelFunction("GetConversationAnalysis")]
        [Description("Get sentiment Analysis of conversation by conversationId")]
        public async Task<string> GetConversationSentimentAsync(string conversationId)
        {
            string conversationApiEndpoint = ENTERPRISE_CONVERSATIONSERVICE_BASEURL;
            Console.WriteLine($"conversationapi: {conversationApiEndpoint}");

            string url = $"{conversationApiEndpoint}/analysis/Conversation/{conversationId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                return $"{{\"error\": \"{error}  {conversationApiEndpoint}\"}}";
            }
        }




    }
}