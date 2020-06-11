using System;
using Microsoft.EntityFrameworkCore;

namespace CloudScale.Data
{
    public class CloudScaleDbContext : DbContext
    {
        public CloudScaleDbContext(DbContextOptions<CloudScaleDbContext> options)
            : base(options)
        {
        }

        // public DbSet<Blog> Blogs { get; set; }
        // public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}