using System;
using System.Collections.Generic;
using Vehicles.Models;

    
namespace Vehicles.Contracts.Responces
{
    public class OwnerResponce
    {
        public string Id{get;set;}
        public string Name{get;set;}
        public string SurName{get;set;}
        public string CarOwnerPhone{get;set;}
        public string Location{get;set;}
        public DateTime BirthDate{get;set;}
        public IEnumerable<CarResponse> CarResponces{get;set;}
    }
}