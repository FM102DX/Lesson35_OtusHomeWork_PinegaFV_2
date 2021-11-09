using Otus.Teaching.Concurrency.Import.Core.Entities;
using System;

namespace Otus.Teaching.Concurrency.Import.Core.Repositories
{
    public interface ICustomerRepository : IDisposable
    {
        public void AddCustomer(Customer customer);

        public void PrintCustomerListToConsole();
        public void PrintCustomerListToFile(string path);

        public void SaveChanges();

        public void PrepareDb();

    }
}