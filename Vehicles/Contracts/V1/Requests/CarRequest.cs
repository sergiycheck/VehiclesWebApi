using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;
using Vehicles.Models;

namespace Vehicles.Contracts.Requests
{
    public class CarRequest:BaseModel
    {
        public string UniqueNumber{get;set;}
        public string Brand{get;set;}
        public string Color{get;set;}
        public DateTime Date{get;set;}
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Price{get;set;}
        [JsonConverter(typeof(FloatToStringConverter))]
        public float CarEngine{get;set;}
        public string Description{get;set;}
        public string Transmision{get;set;}
        public string Drive{get;set;}
        public IEnumerable<OwnerRequest> OwnerRequests {get;set;}

    }
}