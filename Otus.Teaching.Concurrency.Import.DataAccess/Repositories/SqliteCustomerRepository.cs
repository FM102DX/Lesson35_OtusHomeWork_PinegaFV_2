using System;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class SqliteCustomerRepository : ICustomerRepository
    {
        //SQLIte через EF
        private MyContext _context;
        private static readonly object _locker = new object();

        public SqliteCustomerRepository()
        {
            //Создаем репозиторий, проверяем базу, создаем dbcontext
            _context = new MyContext();
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void PrintCustomerListToConsole()
        {
            int counter = 0;
            _context.Customers.ForEachAsync(x=>{
                Console.WriteLine(x.ToString());
                counter++;
            });
            Console.WriteLine($"Обработано {counter} записей");
        }

        public void PrintCustomerListToFile(string path)
        {
            FileWriter file = new FileWriter(path);
            int counter = 0;
            _context.Customers.ForEachAsync(x => {
                file.DoWrite(x.ToString());
                counter++;
            });
            file.DoWrite($"Обработано {counter} записей");
            file.Close();
        }
        

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void PrepareDb()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public class MyContext : DbContext
        {
            public DbSet<Customer> Customers { get; set; }


            public string DbPath { get; private set; }

            public MyContext()
            {
                DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.db");
            }


            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath};");
            }
        }

    }
}