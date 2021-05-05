using System.Collections.Generic;

namespace Vehicles.Contracts.V1.Responses
{
    public class AuthFailedResponse
    {
        public bool ContainsErrors {get;set;}
        public IEnumerable<string> Errors { get; set; }
    }
}