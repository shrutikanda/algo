using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Providers.Azure;
using LangChain.Schema;

var provider = new AzureOpenAiProvider(apiKey: "3995e126ae09492187fc672efaa3e20a", 
endpoint: "https://ldapi-sg1.loandepotdev.works/LLM-POC/", 
deploymentID: "LD-Chat");
var llm = new AzureOpenAiChatModel(provider, id: "LD-Chat");

var result = await llm.GenerateAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result);

//*** Lang Chain

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

var chainResult = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
{
    { "product", "colourful socks" }
}));

Console.WriteLine(chainResult.Value["text"]);
Console.WriteLine("SequentialChain sample finished.");