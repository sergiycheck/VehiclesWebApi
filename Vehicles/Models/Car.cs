using System;
using System.Collections.Generic;

namespace Vehicles.Models
{
    public class Car:BaseModel
    {
        public string UniqueNumber{get;set;}
        public string Brand{get;set;}
        public string Color{get;set;}
        public DateTime Date{get;set;}
        public decimal Price{get;set;}
        public float CarEngine{get;set;}
        public string Description{get;set;}
        public string Transmision{get;set;}
        public string Drive{get;set;}
        public virtual ICollection<ManyToManyCarOwner> ManyToManyCarOwner{get;set;}

    }
}