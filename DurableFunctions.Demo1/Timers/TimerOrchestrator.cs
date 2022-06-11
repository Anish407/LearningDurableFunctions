using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DurableFunctions.Demo1.Timers
{
    public static class TimerOrchestratorDurableFunctions
    {

        // Example from msdocs..  The following example illustrates how to use durable timers for delaying execution.
        // The example is issuing a notification every 1 minute for 10 times.
        // When you create a timer that expires at 4:30 pm,
        // the underlying Durable Task Framework enqueues a message that becomes visible only at 4:30 pm.
        // When running 
        // in the Azure Functions Consumption plan, the newly visible timer
        // message will ensure that the function app gets activated on an appropriate VM.
        [FunctionName("TimerOrchestrator")]
        public static async Task TimerOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            for (int i = 0; i < 10; i++)
            {
                DateTime delayDuration = context.CurrentUtcDateTime.AddSeconds(5);
                await context.CreateTimer(delayDuration, CancellationToken.None);
                await context.CallActivityAsync("TimerActivity", input: $"{i}-Anish");
            }

        }

        [FunctionName("TimerActivity")]
        public static string TimerActivity([ActivityTrigger] string name, ILogger log)
        {
            log.LogError($"Saying hello to {name}.{DateTime.UtcNow}");
            return $"Hello {name}!";
        }


        // using timers for timeouts
        [FunctionName("TryGetQuote")]
        public static async Task<bool> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            DateTime deadline = context.CurrentUtcDateTime.Add(timeout);

            using (var cts = new CancellationTokenSource())
            {
                Task activityTask = context.CallActivityAsync("GetQuote",null);
                Task timeoutTask = context.CreateTimer(deadline, cts.Token);

                Task winner = await Task.WhenAny(activityTask, timeoutTask);
                if (winner == activityTask)
                {
                    // success case
                    cts.Cancel();
                    return true;
                }
                else
                {
                    // timeout case
                    return false;
                }
            }
        }
    }
}