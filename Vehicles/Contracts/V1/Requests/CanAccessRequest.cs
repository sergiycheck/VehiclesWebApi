using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicles.Contracts.V1.Requests
{
    public class CanAccessRequest
    {
        public int? Id { get; set; }
        public string Token { get; set; }
    }
}
