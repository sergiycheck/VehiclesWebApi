using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vehicles.Models;

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
        }
    }
}
