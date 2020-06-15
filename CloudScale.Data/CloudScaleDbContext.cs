using System.Reflection;
using CloudScale.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudScale.Data
{
    public class CloudScaleDbContext : DbContext
    {
        public CloudScaleDbContext(DbContextOptions<CloudScaleDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CloudScaleDbContext).Assembly);
        }
    }
}