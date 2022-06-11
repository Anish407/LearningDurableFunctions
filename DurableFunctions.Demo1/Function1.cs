using DurableFunctions.Demo1.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DurableFunctions.Demo1
{
    public static class Function1
    {
        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var query = HttpUtility.ParseQueryString(req.RequestUri.Query);
            string result = query.Get("name");

            // Function input comes from the request content.
            //string instanceId = await starter.StartNewAsync("SimpleOrchestrator", null, new RequestModel { 
            // Name = result,
            //});

            string instanceId = await starter.StartNewAsync("HumanInteractionOrchestrator");

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}