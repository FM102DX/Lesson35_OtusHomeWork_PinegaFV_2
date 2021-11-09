using Otus.Teaching.Concurrency.Import.Core.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Otus.Teaching.Concurrency.Import.Core.Repositories
{
    public interface IGenericRepository<T>
    {

        //универсальный репозиторий, использующий модель сохранения EF,
        // когда мы сначала вносим изменения, потом вызываем SaveChanges() чтобы все сохранить в базу
        //подробнее https://metanit.com/sharp/entityframework/3.13.php

        public long Count { get; }

        public string Guid { get; }
        public string DbContextGuid { get; }

        public Task<CommonOperationResult> AddItemAsync(T t);

        public CommonOperationResult AddItem(T t);

        public CommonOperationResult UpdateItem(T t);

        public CommonOperationResult Remove(T t);

        public T GetItemById(int id);

        public IEnumerable<T> GetAllItems();

        public void PrintItemListToConsole();

        public void PrintItemListToFile(string path);

        public void SaveChanges();

        public void PrepareDb();
    }
}
