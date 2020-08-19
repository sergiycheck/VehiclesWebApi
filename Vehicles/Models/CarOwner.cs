using System;
using System.Collections.Generic;

namespace Vehicles.Models
{
    public class CarOwner:BaseModel
    {
        public string Name{get;set;}
        public string SurName{get;set;}
        public string  Patronymic{get;set;}
        public string CarOwnerPhone{get;set;}
        public string Location{get;set;}
        public DateTime BirthDate{get;set;}
        public virtual ICollection<ManyToManyCarOwner> ManyToManyCarOwner{get;set;}
    }
    public class ManyToManyCarOwner
    {
        public int CarId{get;set;}
        public Car Car{get;set;}
        public int CarOwnerId{get;set;}
        public CarOwner CarOwner{get;set;}
    }
}