using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

          public virtual DbSet<Book> Books { get; set; }
          public virtual DbSet<Customer> Customers { get; set; }
          public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<BookStore.Models.ClusterResulter> ClusterResulter { get; set; }

    }


}

