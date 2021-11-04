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
        public bool ForSale { get; set; }
        public string Drive{get;set;}
        public IEnumerable<OwnerRequest> OwnerRequests {get;set;}

        public string Token { get; set; }

        public string UpdateImage { get; set; }

        public override string ToString()
        {
            var forSale = this.ForSale ? "true" : "false";
            return $"{Brand} {this.Color} {this.CarEngine} {this.Date} {this.Description} {this.Drive} {forSale} {this.Id} {this.Price}  {this.Transmision} {this.UniqueNumber} {this.UpdateImage} ";
        }
    }
}