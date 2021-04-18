using System;
using System.Collections.Generic;
using Vehicles.Models;

    
namespace Vehicles.Contracts.Requests
{
    public class OwnerRequest
    {
        public string Id {get;set;}
        public string Name{get;set;}
        public string UserName { get; set; }
        public string SurName{get;set;}
        public string CarOwnerPhone{get;set;}
        public string Location{get;set;}
        public DateTime BirthDate{get;set;}
        public IEnumerable<CarRequest> CarRequests{get;set;}
    }
}