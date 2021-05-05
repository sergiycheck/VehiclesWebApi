using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Models;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;

namespace vehicles.Models
{
    public class Penalty: BaseModel
    {
        public bool PayedStatus { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Price { get; set; }
        public DateTime Date { get; set; }

    }
}
