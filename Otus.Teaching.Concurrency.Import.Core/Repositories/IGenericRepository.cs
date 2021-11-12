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

        public bool Exists(int id);

        public string Guid { get; }

        public string DbContextGuid { get; }

        public CommonOperationResult AddItem(T t);

        public CommonOperationResult UpdateItem(T t);

        public CommonOperationResult Delete(int id);

        public T GetItemByIdOrNull(int id);

        public IEnumerable<T> GetAllItems();

        public void PrintItemListToConsole();

        public void PrintItemListToFile(string path);

        public void PrepareDb();
    }
}
