using System;

namespace DurableFunctions.Demo1.Models
{
    public class SimpleActivityResponseModel
    {
        public string Message { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public bool  IsComplete { get; set; }

    }
}
