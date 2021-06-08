using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Models;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;
using Vehicles.Contracts.Responces;

namespace vehicles.Contracts.V1.Responses
{
    public class PenaltyResponse : BaseModel
    {
        public bool PayedStatus { get; set; }
        public string CarUniqueNumber { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }
}
