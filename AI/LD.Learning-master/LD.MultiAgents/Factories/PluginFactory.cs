using LD.MultiAgents.AgentPlugins;
using LD.MultiAgents.Models;
using Microsoft.SemanticKernel;

namespace LD.MultiAgents.Factories
{
    internal static class PluginFactory
    {
        internal static Kernel GetAgentKernel(Kernel kernel, AgentType agentType, ILoggerFactory loggerFactory, string userId)
        {
            Kernel agentKernel = kernel.Clone();
            switch (agentType)
            {
                case AgentType.Sales:
                    var salesPlugin = new SalesPlugin(loggerFactory.CreateLogger<SalesPlugin>(), userId);
                    agentKernel.Plugins.AddFromObject(salesPlugin);
                    break;
             
                case AgentType.Coordinator:
                    var CoordinatorPlugin = new CoordinatorPlugin(loggerFactory.CreateLogger<CoordinatorPlugin>(),  userId);
                    agentKernel.Plugins.AddFromObject(CoordinatorPlugin);
                    break;

                case AgentType.ConversationAnalysis:
                    var ConversationAnalysisPlugin = new ConversationAnalysisPlugin(loggerFactory.CreateLogger<ConversationAnalysisPlugin>(), userId);
                    agentKernel.Plugins.AddFromObject(ConversationAnalysisPlugin);
                    break;
                case AgentType.LoanDepot:
                    var LoanDepotPlugin = new LoanDepotPlugin(loggerFactory.CreateLogger<LoanDepotPlugin>(), userId);
                    agentKernel.Plugins.AddFromObject(LoanDepotPlugin);
                    break;
                default:
                    throw new ArgumentException("Invalid plugin name");
            }

            return agentKernel;
        }
    }
}
