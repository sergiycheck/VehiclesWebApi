using System;
using System.Collections.Generic;

namespace Vehicles.Models
{
    // public class CarOwner:BaseModel
    // {
    //     public string Name{get;set;}
    //     public string SurName{get;set;}
    //     public string  Patronymic{get;set;}
    //     public string CarOwnerPhone{get;set;}
    //     public string Location{get;set;}
    //     public DateTime BirthDate{get;set;}
        
    // }
    public class ManyToManyCustomUserToVehicle
    {
        public int CarId{get;set;}
        public Car Car{get;set;}
        public string CarOwnerId{get;set;}
        public CustomUser CarOwner{get;set;}
    }
}