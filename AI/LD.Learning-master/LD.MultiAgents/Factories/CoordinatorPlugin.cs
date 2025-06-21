using LD.MultiAgents.AgentPlugins;
using Microsoft.SemanticKernel;

namespace LD.MultiAgents.Factories
{
    public class CoordinatorPlugin : BasePlugin
    {

        public CoordinatorPlugin(ILogger<BasePlugin> logger, string userId)
           : base(logger, userId)
        {
        }
    }
}
