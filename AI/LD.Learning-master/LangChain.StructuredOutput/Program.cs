using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;

public class SmsService
{

    static public ChatTool GetTool()
    {
        ChatTool tool = ChatTool.CreateFunctionTool(
            nameof(SendSms),
            "send SMS",
            BinaryData.FromString(
                @"
                {
                    ""type"": ""object"",
                    ""properties"": {
                        ""phone_number"": {
                            ""type"": ""string"",
                            ""description"": ""The phone number to send the SMS to""
                        },
                        ""message"": {
                            ""type"": ""string"",
                            ""description"": ""The message to send in the SMS""
                        }
                    },
                    ""required"": [""phone_number"", ""message""]
                }
                "));

        return tool;

    }

    public static string SendSms(string phoneNumber, string message)
    {
        // Dummy response for now
        var sms = new
        {
            phone_number = phoneNumber,
            message,
            status = "Sent"
        };

        return JsonSerializer.Serialize(sms);
    }

}

public class ChatServiceFunctionCalling
{

    public async Task<string> GetReponse(List<ChatMessageViewModel> mesages)
    {
        AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri("https://ldapi-sg1.loandepotdev.works/LLM-POC/"),
         new System.ClientModel.ApiKeyCredential("3995e126ae09492187fc672efaa3e20a"));

        ChatClient chatClient = azureClient.GetChatClient("LD-Chat");

        var systemPrompt = "You are an expert , you ask for sms";

        var systemMessage = new SystemChatMessage(systemPrompt);

        List<ChatMessage> chatMessages = new List<ChatMessage>();
        chatMessages.Add(systemMessage);

        foreach (var mesage in mesages)
        {
            if(mesage.Role == "user")
            {
                chatMessages.Add(new UserChatMessage(mesage.Message));
            }
            else
            {
                chatMessages.Add(new AssistantChatMessage(mesage.Message));

            }

        }

        var options = new ChatCompletionOptions()
        {
            Tools = { SmsService.GetTool() }
        };

        ChatCompletion completion = chatClient.CompleteChat(chatMessages, options);

        if (completion.FinishReason == ChatFinishReason.ToolCalls)
        {
            var toolCall = completion.ToolCalls[0];

            if (toolCall.FunctionName == "SendSms")
            {
                var arguments = toolCall.FunctionArguments.ToString();
                var argsDoc = JsonDocument.Parse(arguments); // Parse the stringified JSON
                if (argsDoc.RootElement.TryGetProperty("phone_number", out JsonElement phoneNumberElement) && phoneNumberElement.ValueKind == JsonValueKind.String)
                {
                    string phoneNumber = phoneNumberElement.GetString();
                    string message = argsDoc.RootElement.TryGetProperty("message", out var messageProp) ? messageProp.GetString() : "Hello from Azure!";

                    string toolResult = SmsService.SendSms(phoneNumber, message);

                    return toolResult;
                }
                else
                {
                    Console.WriteLine("Invalid JSON structure for 'phone_number' property.");

                }
            }
         


        }

        return "test";
    }
public class ChatMessageViewModel
{
    public string Role { get; set; }
    public string Message { get; set; }
}


    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var chatService = new ChatServiceFunctionCalling();

            var messages = new List<ChatMessageViewModel>
    {
        new ChatMessageViewModel
        {
            Role = "user",
            Message = "Please send an SMS to +1234567890 saying Hello from Azure!"
        }
    };

            string response = await chatService.GetReponse(messages);
            Console.WriteLine("=== Chat Completion Response ===");
            Console.WriteLine(response);


            //*************************
            //   string endpoint = "https://ldapi-sg1.loandepotdev.works/LLM-POC/";
            //   string apiKey = "3995e126ae09492187fc672efaa3e20a";
            //   string deployment = "LD-Chat";
            //   string apiVersion = "2024-02-15-preview";

            //   var url = $"{endpoint}/openai/deployments/{deployment}/chat/completions?api-version={apiVersion}";

            //   httpClient.DefaultRequestHeaders.Clear();
            //   httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            //   httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            //   var request = new
            //   {
            //       messages = new object[]
            //       {
            //           new { role = "user", content = "What’s the weather in Paris today?" }
            //       },
            //       functions = new[]
            //       {
            //           new
            //           {
            //               name = "get_current_weather",
            //               description = "Get the current weather in a given location",
            //               parameters = new
            //               {
            //                   type = "object",
            //                   properties = new
            //                   {
            //                       location = new { type = "string", description = "The city name" },
            //                       unit = new
            //                       {
            //                           type = "string",
            //                           @enum = new[] { "celsius", "fahrenheit" }
            //                       }
            //                   },
            //                   required = new[] { "location" }
            //               }
            //           }
            //       },
            //       function_call = "auto",
            //       temperature = 0.7
            //   };

            //   var jsonRequest = JsonSerializer.Serialize(request);
            //   var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            //   Console.WriteLine("Calling Azure OpenAI function...");
            //   var response = await httpClient.PostAsync(url, content);
            //   var responseBody = await response.Content.ReadAsStringAsync();

            //   Console.WriteLine("\n== Raw Response ==");
            //   Console.WriteLine(responseBody);

            //   // Parse the function call
            //   var doc = JsonDocument.Parse(responseBody);
            //   var functionCall = doc.RootElement
            //.GetProperty("choices")[0]
            //.GetProperty("message")
            //.GetProperty("function_call");

            //   var functionName = functionCall.GetProperty("name").GetString();
            //   var argumentsJson = functionCall.GetProperty("arguments").GetString(); // Get the string value

            //   Console.WriteLine($"\n== Parsed Function Call ==");
            //   Console.WriteLine($"Function: {functionName}");
            //   Console.WriteLine($"Arguments: {argumentsJson}");

            //   if (functionName == "get_current_weather")
            //   {
            //       var argsDoc = JsonDocument.Parse(argumentsJson); // Parse the stringified JSON
            //       if (argsDoc.RootElement.TryGetProperty("location", out JsonElement locationElement) && locationElement.ValueKind == JsonValueKind.String)
            //       {
            //           string location = locationElement.GetString();
            //           string unit = argsDoc.RootElement.TryGetProperty("unit", out var unitProp) ? unitProp.GetString() : "celsius";

            //           string toolResult = GetCurrentWeather(location, unit);

            //           Console.WriteLine($"\n== Tool Executed ==");
            //           Console.WriteLine(toolResult);
            //       }
            //       else
            //       {
            //           Console.WriteLine("Invalid JSON structure for 'location' property.");
            //       }
            //   }


        }

        public static string GetCurrentWeather(string location, string unit)
        {
            // Dummy response for now
            var weather = new
            {
                location,
                temperature = unit == "fahrenheit" ? "75°F" : "24°C",
                condition = "Sunny"
            };

            return JsonSerializer.Serialize(weather);
        }
    }
}
