using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vehicles.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using vehicles.Models;

namespace Vehicles.Data
{
    public class VehicleDbContext : IdentityDbContext<CustomUser, CustomRole, string>
    {
        public DbSet<Car> Cars{get;set;}
        
        public DbSet<ManyToManyCustomUserToVehicle> ManyToManyCarOwners{get;set;}
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();//for initializing initial data//for testing
        }

        //dotnet ef migrations add "InitialCreate"

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.Entity<ManyToManyCustomUserToVehicle>()
                .HasKey(mtm=>new{mtm.CarOwnerId,mtm.CarId});

            builder.Entity<ManyToManyCustomUserToVehicle>()
                .HasOne(co=>co.CarOwner)
                .WithMany(mtm=>mtm.ManyToManyCustomUserToVehicle)
                .HasForeignKey(co=>co.CarOwnerId);

            builder.Entity<ManyToManyCustomUserToVehicle>()
                .HasOne(c=>c.Car)
                .WithMany(mtm=>mtm.ManyToManyCustomUserToVehicle)
                .HasForeignKey(c=>c.CarId);
                
            builder.Entity<Car>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(builder);

            //alternative way for data seeding that is better for testing
            //remove other ways of db initializing
            //InitializeDb(builder);
        }
        private void InitializeDb(ModelBuilder builder) 
        {
            // var seed = new SeedData();
            // builder.Entity<Car>().HasData(seed.Cars);
            // var customUsers =new List<CustomUser>();
            // seed.UsersAndPasswords.ToList().ForEach(usrPass=>{
            //     customUsers.Add(usrPass.CustomUser);
            // });
            // builder.Entity<CustomUser>().HasData(customUsers);
            // builder.Entity<ManyToManyCustomUserToVehicle>().HasData(seed.GetManyToManyCustomUserToVehicle());
        }
    }
}
