using DurableFunctions.Demo1.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DurableFunctions.Demo1
{
    public class Orchestrators
    {
        public static int counter { get; set; } = 1;

        [FunctionName(nameof(SimpleOrchestrator))]
        public static async Task<List<SimpleActivityResponseModel>> SimpleOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            // use this method if logging is needed inside an orchestrator, this will take care of replaying
            logger = context.CreateReplaySafeLogger(logger);

            RequestModel request = context.GetInput<RequestModel>();

            List<SimpleActivityResponseModel> outputs = new List<SimpleActivityResponseModel>();

            // managing replaying inside the orchestrator 
            if(!context.IsReplaying) counter += 1;


            logger.LogInformation("Calling activity : 1");
            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<SimpleActivityResponseModel>("SimpleActivity", new SimpleActivityRequestModel
            {
                data = request,
                State = "Tokyo"
            }));

            logger.LogInformation("Calling activity : 2");
            outputs.Add(await context.CallActivityAsync<SimpleActivityResponseModel>("SimpleActivity", new SimpleActivityRequestModel
            {
                data = request,
                State = "Seattle"
            }));

            logger.LogInformation("Calling activity : 3");
            outputs.Add(await context.CallActivityAsync<SimpleActivityResponseModel>("SimpleActivity", new SimpleActivityRequestModel
            {
                data = request,
                State = "London"
            }));

            return outputs;
        }
    }
}
