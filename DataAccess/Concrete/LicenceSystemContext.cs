using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete
{
    public class LicenseSystemContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost; Database=LicenseSystem; Integrated Security=True;");
        }

        public DbSet<KeyLicense>? KeyLicenses { get; set; }
        public DbSet<Log>? Logs { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<OperationClaim>? OperationClaims { get; set; }
        public DbSet<UserOperationClaim>? UserOperationClaims { get; set; }
        public DbSet<Panel>? Panels { get; set; }
        public DbSet<Application>? Applications { get; set; }
    }
}