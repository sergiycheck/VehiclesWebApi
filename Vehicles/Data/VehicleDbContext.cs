using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vehicles.Models;
using Vehicles.Data;

namespace Vehicles.Data
{
    public class VehicleDbContext : DbContext
    {
        public DbSet<Car> Cars{get;set;}
        public DbSet<CarOwner> CarOwners{get;set;}
        public DbSet<ManyToManyCarOwner> ManyToManyCarOwners{get;set;}
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();//for initializing initial data//for testing
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);
            builder.Entity<ManyToManyCarOwner>()
                .HasKey(mtm=>new{mtm.CarOwnerId,mtm.CarId});

            // builder.Entity<ManyToManyCarOwner>()
            //     .HasOne(co=>co.CarOwner)
            //     .WithMany(mtm=>mtm.ManyToManyCarOwner)
            //     .HasForeignKey(co=>co.CarOwnerId);

            // builder.Entity<ManyToManyCarOwner>()
            //     .HasOne(c=>c.Car)
            //     .WithMany(mtm=>mtm.ManyToManyCarOwner)
            //     .HasForeignKey(c=>c.CarId);
                
            builder.Entity<Car>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,4)");

            //alternative way for data seeding that is better for testing
            //remove other ways of db initializing
            //InitializeDb(builder);
        }
        private void InitializeDb(ModelBuilder builder) 
        {
            var seed = new SeedData();
            builder.Entity<Car>().HasData(seed.Cars);
            builder.Entity<CarOwner>().HasData(seed.CarOwners);
            builder.Entity<ManyToManyCarOwner>().HasData(seed.ManyToManyCarOwners);
        }
    }
}
