using System;
using Microsoft.AspNetCore.Identity;
using Vehicles.Models;
using System.Collections.Generic;

namespace Vehicles.Models
{
    public class CustomUser:IdentityUser
    {
        //seed user using provider
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string Country{get;set;}
        public string City{get;set;}
        public string Address{get;set;}
        public DateTime BirthDate{get;set;}

        public virtual ICollection<ManyToManyCustomUserToVehicle> ManyToManyCustomUserToVehicle{get;set;}


        //implemented properties
        //public virtual TKey Id { get; set; }
        //public virtual string PhoneNumber { get; set; }
        //public virtual bool EmailConfirmed { get; set; 
        // public virtual string Email { get; set; }
        //public virtual string UserName { get; set; }
    }
}