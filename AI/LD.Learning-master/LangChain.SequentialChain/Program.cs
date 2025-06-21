using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Prompts;
using LangChain.Schema;
using LangChain.Providers.Azure;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Base;

using var httpClient = new HttpClient();
// Pull the API key from the environment, so it's never checked in with source
var provider = new AzureOpenAiProvider(apiKey: "3995e126ae09492187fc672efaa3e20a",
endpoint: "https://ldapi-sg1.loandepotdev.works/LLM-POC/",
deploymentID: "LD-Chat");
var llm = new AzureOpenAiChatModel(provider, id: "LD-Chat");

var firstTemplate = "What is a good name for a company that makes {product}?";
var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1) { "product" }));

var chainOne = new LlmChain(new LlmChainInput(llm, firstPrompt)
{
    Verbose = true,
    OutputKey = "company_name"
});

var secondTemplate = "Write a 20 words description for the following company:{company_name}";
var secondPrompt = new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1) { "company_name" }));

var chainTwo = new LlmChain(new LlmChainInput(llm, secondPrompt));

var overallChain = new SequentialChain(new SequentialChainInput(
    new IChain[]
    {
        chainOne,
        chainTwo
    },
    new[] { "product" },
    new[] { "company_name", "text" }
));

var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
{
    { "product", "colourful socks" }
}), new LoggingCallbacks());

Console.WriteLine(result.Value["text"]);
Console.WriteLine("SequentialChain sample finished.");

return;


public class LoggingCallbacks : ICallbacks
{
    public Task OnChainStart(BaseChain chain, IChainValues values)
    {
        Console.WriteLine($"Chain started: {chain.ChainType()} with input: {values}");
        return Task.CompletedTask;
    }

    public Task OnChainEnd(BaseChain chain, IChainValues values, IChainValues result)
    {
        Console.WriteLine($"Chain ended: {chain.ChainType()} with result: {result}");
        return Task.CompletedTask;
    }

    public Task OnChainError(BaseChain chain, Exception exception, IChainValues values)
    {
        Console.WriteLine($"Chain error: {chain.ChainType()} with exception: {exception.Message}");
        return Task.CompletedTask;
    }
}

