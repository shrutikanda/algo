using Azure.AI.OpenAI;
using Azure;

namespace AI.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            var endpoint = new Uri("https://ld-llm-openai-poc.openai.azure.com");
            var apiKey = "c94f64b7a8e14874b94313a622463f4c";
            var credentials = new AzureKeyCredential(apiKey);

            var client = new OpenAIClient(endpoint, credentials);

          var completion=  client.GetChatCompletions("LD-Chat", new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.User, "What is a good name for a company that sells colourful socks?")
                },
                Temperature = 0.7f,
                MaxTokens = 100
            }).Value;


            var AI = completion.Choices?.FirstOrDefault()?.Message;

        }
    }
}