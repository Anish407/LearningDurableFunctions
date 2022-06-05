using System;

namespace DurableFunctions.Demo1.Models
{
    public class RequestModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid(); 
    }
}
