using Microsoft.EntityFrameworkCore;
using Vehicles.Data;
using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace VehiclesXUnitTests
{
    public class SqlServerTests:IDisposable
    {
        //public const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=VehiclesWebApiDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        public const string ConnectionString = "Server=DESKTOP-SH1NLDB\\SQLEXPRESS;Database=VehiclesWebApiDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        //Data Source=DESKTOP-SH1NLDB\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False



        public SqlServerTests()
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
        }
        public DbConnection Connection { get; }
        public VehicleDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new VehicleDbContext
                (new DbContextOptionsBuilder<VehicleDbContext>()
                .UseSqlServer(Connection)
                .Options);
            if (transaction != null) 
            {
                context.Database.UseTransaction(transaction);
            }
            return context;
        }
        public void Dispose() => Connection.Dispose();
    }
}