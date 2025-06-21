using Azure.AI.OpenAI;
using AzureOpenAI.Chat.Services;
using System.Text.Json;

public class FunctionOrchestrator
{
    private readonly Dictionary<string, Func<JsonElement, Task<string>>> _functions;

    public FunctionOrchestrator()
    {
        _functions = new Dictionary<string, Func<JsonElement, Task<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["GetLoanInformationByEmpowerLoanIdAsync"] = async args =>
            {
                var id = args.GetProperty("empowerLoanId").GetString();
                return await DCRM.GetLoanInformationByEmpowerLoanIdAsync(id!);
            },
            ["GetCustomerServiceInformationByEmailAsync"] = async args =>
            {
                var email = args.GetProperty("email").GetString();
                return await DCRM.GetCustomerServiceInformationByEmailAsync(email!);
            },
            ["GetConversationSentimentAsync"] = async args =>
            {
                var convId = args.GetProperty("conversationId").GetString();
                return await DCRM.GetConversationSentimentAsync(convId!);
            }
        };
    }

    public bool TryExecuteFunction(FunctionCall functionCall, out Task<string> resultTask)
    {
        if (_functions.TryGetValue(functionCall.Name, out var func))
        {
            var args = JsonDocument.Parse(functionCall.Arguments).RootElement;
            resultTask = func(args);
            return true;
        }

        resultTask = Task.FromResult("Unknown function.");
        return false;
    }
}
