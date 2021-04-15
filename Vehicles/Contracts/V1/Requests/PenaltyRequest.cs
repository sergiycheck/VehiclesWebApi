using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Models;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;
using Vehicles.Contracts.Responces;
using Vehicles.Contracts.Requests;

namespace vehicles.Contracts.V1.Responses
{
    public class PenaltyPayRequest 
    {
        public int? Id { get; set; }
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
    }

    public class PenaltyRequest : BaseModel
    {
        public bool PayedStatus { get; set; }
        public CarRequest Car { get; set; }
        public int? CarId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string Token { get; set; }
    }
}
