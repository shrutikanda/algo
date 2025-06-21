using Azure.AI.OpenAI;
using System.Text.Json;

public static class FunctionCallingTools
{
    public static FunctionDefinition GetCustomerServiceTool()
    {
        return new FunctionDefinition()
        {
            Name = "GetCustomerServiceInformationByEmailAsync",
            Description = "Get customer service information based on email.",
            Parameters = BinaryData.FromString(@"
                {
                    ""type"": ""object"",
                    ""properties"": {
                        ""email"": {
                            ""type"": ""string"",
                            ""description"": ""The email address of the customer to retrieve information for.""
                        }
                    },
                    ""required"": [""email""]
                }
            ")
        };
    }

    public static FunctionDefinition GetLoanInformationTool()
    {
        return new FunctionDefinition()
        {
            Name = "GetLoanInformationByEmpowerLoanIdAsync",
            Description = "Get loan information based on Empower Loan ID.",
            Parameters = BinaryData.FromString(@"
                {
                    ""type"": ""object"",
                    ""properties"": {
                        ""empowerLoanId"": {
                            ""type"": ""string"",
                            ""description"": ""The Empower Loan ID used to look up the loan.""
                        }
                    },
                    ""required"": [""empowerLoanId""]
                }
            ")
        };
    }

    public static FunctionDefinition GetConversationSentimentTool()
    {
        return new FunctionDefinition()
        {
            Name = "GetConversationSentimentAsync",
            Description = "Get sentiment analysis result of a conversation based on its ID.",
            Parameters = BinaryData.FromString(@"
                {
                    ""type"": ""object"",
                    ""properties"": {
                        ""conversationId"": {
                            ""type"": ""string"",
                            ""description"": ""The ID of the conversation to analyze for sentiment.""
                        }
                    },
                    ""required"": [""conversationId""]
                }
            ")
        };
    }
}
