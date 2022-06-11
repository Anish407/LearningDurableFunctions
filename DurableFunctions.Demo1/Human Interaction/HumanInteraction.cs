using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctions.Demo1.Human_Interaction
{
    public static class HumanInteraction
    {
        [FunctionName(nameof(HumanInteractionOrchestrator))]
        public static async Task HumanInteractionOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            string result = "na";

            result = await context.WaitForExternalEvent<string>("passorfail");

            logger.LogError(result);
        }

       
    }
}