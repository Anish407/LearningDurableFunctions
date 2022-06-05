﻿using DurableFunctions.Demo1.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;

namespace DurableFunctions.Demo1
{
    public class Activities
    {
        [FunctionName(nameof(SimpleActivity))]
        public static SimpleActivityResponseModel SimpleActivity([ActivityTrigger] SimpleActivityRequestModel requestModel, ILogger log)
        {
            log.LogInformation($"Saying hello to {requestModel.State}.");
            return new SimpleActivityResponseModel
            {
                DateTime = DateTimeOffset.UtcNow,
                Message = $"{requestModel.data.Id}-{requestModel.data.Name}-{requestModel.State}"
            };
        }
    }
}