using LD.MultiAgents.Models;
using static LD.MultiAgents.StructuredFormats.ChatResponseFormatBuilder;

namespace LD.MultiAgents.Factories
{
    internal static class SystemPromptFactory
    {
        public static string GetAgentName(AgentType agentType)
        {

            string name = string.Empty;
            switch (agentType)
            {
                case AgentType.Sales:
                    name = "Sales";
                    break;                              
                case AgentType.Coordinator:
                    name = "Coordinator";
                    break;
                case AgentType.ConversationAnalysis:
                    name = "ConversationAnalysis";
                    break;
                case AgentType.LoanDepot:
                    name = "LoanDepot";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
            }



            return name;//.ToUpper();
        }


        public static string GetAgentPrompts(AgentType agentType)
        {

            string promptFile = string.Empty;
            switch (agentType)
            {
                case AgentType.Sales:
                    promptFile = "Sales.prompty";
                    break;              
             
                case AgentType.Coordinator:
                    promptFile = "Coordinator.prompty";
                    break;
                case AgentType.ConversationAnalysis:
                    promptFile = "ConversationAnalysis.prompty";
                    break;
                case AgentType.LoanDepot:
                    promptFile = "LoanDepot.prompty";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
            }

            string prompt = $"{File.ReadAllText("Prompts/" + promptFile)}{File.ReadAllText("Prompts/CommonAgentRules.prompty")}";

            return prompt;
        }

        public static string GetStratergyPrompts(ChatResponseStrategy strategyType)
        {
            string prompt = string.Empty;
            switch (strategyType)
            {
                case ChatResponseStrategy.Continuation:
                    prompt = File.ReadAllText("Prompts/SelectionStrategy.prompty");
                    break;
                case ChatResponseStrategy.Termination:
                    prompt = File.ReadAllText("Prompts/TerminationStrategy.prompty");
                    break;

            }
            return prompt;
        }
    }
}
