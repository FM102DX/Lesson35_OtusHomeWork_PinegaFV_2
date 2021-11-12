using Microsoft.EntityFrameworkCore;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Core.Service;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Data
{
    public class OtusMultiTreadDbContext : DbContext
    {
        public string DbPath { get; private set; }

        public string Guid { get; private set; }

        public OtusMultiTreadDbContext()
        {
            DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyOtusObjects.db");
            Guid = ServiceFunctions.generateNBlockGUID(1, 4);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            /*
             * 
             * modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.FullName).HasMaxLength(30);
            });

            */

            modelBuilder.Entity<ConsoleToApiMessage>().Property(b => b.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Customer>().Property(b => b.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }


    }
}
