using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;

namespace Vehicles.Models
{
    public class BaseModel
    {
        [JsonConverter(typeof(IntToStringConverter))]
        public int Id{get;set;}
    }
    
}