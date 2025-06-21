using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace LD.MultiAgents.AgentPlugins
{

    public class BasePlugin
    {
        protected readonly ILogger<BasePlugin> _logger;

        protected readonly string _userId;
        

        public BasePlugin(ILogger<BasePlugin> logger,  string userId)
        {
            _logger = logger;
            
            _userId = userId;
        }

        [KernelFunction("GetCurrentDateTime")]
        [Description("Get the current date time in UTC")]
        public DateTime GetCurrentDateTime()
        {
            _logger.LogTrace($"Get Datetime: {System.DateTime.Now.ToUniversalTime()}");
            return System.DateTime.Now.ToUniversalTime();
        }


    }
}


