using DurableFunctions.Demo1.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
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
            if (!context.IsReplaying) counter += 1;


            logger.LogInformation("Calling activity : 1");

            try
            {
                // retry 4 times with a delay of 2 secs in each retry
                RetryOptions retryOptions = new RetryOptions(
                                     firstRetryInterval: TimeSpan.FromSeconds(2),
                                     maxNumberOfAttempts: 4);

                SimpleActivityResponseModel responseFromActivity1 = await context.CallActivityWithRetryAsync<SimpleActivityResponseModel>(
                       "SimpleActivity",
                       
                       retryOptions,
                       input: new SimpleActivityRequestModel
                       {
                           data = request,
                           State = "Tokyo"
                       });

                // either handle exceptions inside the activities like below
                // or wrap the orchestrator with a try catch block
                //if (!responseFromActivity1.IsComplete)
                //{
                //    // run cleanup activity
                //    throw new Exception("");
                //}
                //else
                //{
                outputs.Add(responseFromActivity1);


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
            }
            catch (Exception ex)
            {
                // run the cleanup activity 
                return new List<SimpleActivityResponseModel>
                {

                };
            }
            //}
            // Use this option when we want to add retry logic while calling an activity
           

            return outputs;
        }
    }
}
