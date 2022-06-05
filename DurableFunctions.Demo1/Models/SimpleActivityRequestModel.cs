using System;

namespace DurableFunctions.Demo1.Models
{
    public class SimpleActivityRequestModel
    {
        public RequestModel data { get; set; }
       
        public string State { get; set; }

    }

}
