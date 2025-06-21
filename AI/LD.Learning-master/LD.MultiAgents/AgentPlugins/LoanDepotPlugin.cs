using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.Extensions.Logging;

namespace LD.MultiAgents.AgentPlugins
{
    public class LoanDepotPlugin : BasePlugin
    {
        protected readonly ILogger<BasePlugin> _logger;
        protected readonly string _userId;
        private readonly IChromaService _chromaService;
        private readonly Kernel _kernel;

        

        public LoanDepotPlugin(ILogger<BasePlugin> logger, string userId)
       : base(logger, userId)
        {
            _logger = logger;
            _userId = userId;
            _chromaService = new ChromaService();
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
        }

        [KernelFunction("GetLoanDepotTeamInformation")]
        [Description("Get Information about LoanDepot teams")]
        public async Task<List<string>> GetDCRMTeamInformation(string query)
        {
            _logger.LogTrace($"Get Datetime: {System.DateTime.Now.ToUniversalTime()}");

            var embeddingModel = _kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();

            var embedding = await embeddingModel.GenerateEmbeddingAsync(query);

            // Convert ReadOnlyMemory<float> to IList<float>
            var queryEmbedding = embedding.ToArray();
            var result = await _chromaService.QueryAsync("DCRM-knowledge-base", queryEmbedding, 3);

            return result;

        }
    }
}
