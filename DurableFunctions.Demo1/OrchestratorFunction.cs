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
                string[] states = await context.CallSubOrchestratorAsync<string[]>("GetInitialConfigValuesSubOrchestrator", null);


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
                           State = states[0]
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
                    State = states[1]
                }));

                logger.LogInformation("Calling activity : 3");
                outputs.Add(await context.CallActivityAsync<SimpleActivityResponseModel>("SimpleActivity", new SimpleActivityRequestModel
                {
                    data = request,
                    State = states[2]
                }));
            }
            catch (Exception ex)
            {
                // run the cleanup activity 
                return new List<SimpleActivityResponseModel>
                {
                    new SimpleActivityResponseModel
                    {
                        IsComplete= false,
                        Message= ex.Message
                    }
                };
            }
            //}
            // Use this option when we want to add retry logic while calling an activity
           

            return outputs;
        }

        [FunctionName(nameof(GetInitialConfigValuesSubOrchestrator))]
        public static async Task<string[]> GetInitialConfigValuesSubOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
           var result =  await  context.CallActivityAsync<string[]>("GetConfigActivity", null);

            return result;
        }
    }
}
