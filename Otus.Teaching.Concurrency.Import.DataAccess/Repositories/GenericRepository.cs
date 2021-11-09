using System;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using System.Threading.Tasks;
using System.Linq;
using Otus.Teaching.Concurrency.Import.Core.Abstract;
using Otus.Teaching.Concurrency.Import.Core.Service;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : KeepableClass, IKeepable
    {
        //SQLIte через EF
        protected OtusMultiTreadDbContext _context;
        private static readonly object _locker = new object();

        public string Guid {get; private set;}

        public GenericRepository(OtusMultiTreadDbContext context)
        {
            //Создаем репозиторий, проверяем базу, создаем dbcontext
            _context = context;
            Guid = ServiceFunctions.generateNBlockGUID(1, 4);
        }

        public long Count 
        { 
            get 
            { 
                return _context.Set<T>().Count();
            } 
        }

        public IEnumerable<T> GetAllItems()
        {
            return GetAllAsync().Result;
                //_dbSet.AsNoTracking().ToListAsync().Result;
        }

        public IEnumerable<T> GetItemsFiltered(Func<T, bool> predicate)
        {
            return null;
            //return _dbSet.AsNoTracking(). Where(predicate).ToList();
        }

        public T GetItemById(int id)
        {
            // return _dbSet.Find(id);

            return GetByIdOrNullAsync(id).Result;
        }


        public string DbContextGuid { get { return _context.Guid; }}
        public async Task<T> GetByIdOrNullAsync(int id)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<CommonOperationResult> AddItemAsync(T t)
        {
            Console.WriteLine("Adding item async");

            try
            {
                await _context.Set<T>().AddAsync(t);
               int rez = await _context.SaveChangesAsync();

                return CommonOperationResult.sayOk(rez.ToString());
            }
            catch (Exception ex)
            {
                return CommonOperationResult.sayFail(ex.Message);
            }

        }

        public CommonOperationResult AddItem(T t)
        {
            Console.WriteLine("Adding item");
            try
            {
                _context.Set<T>().Add(t);
                _context.SaveChanges();
                return CommonOperationResult.sayOk();
            }
            catch (Exception ex)
            {
                return CommonOperationResult.sayFail(ex.Message); 
            }
        }

        public CommonOperationResult UpdateItem(T t)
        {
            //_context.Entry(t).State = EntityState.Modified;
            _context.Set<T>().Update(t);
            var rez=_context.SaveChanges();
            return CommonOperationResult.sayOk(rez.ToString());
        }
        public CommonOperationResult Remove(T t)
        {
            _context.Set<T>().Remove(t);
            var rez = _context.SaveChanges();
            return CommonOperationResult.sayOk(rez.ToString());
        }

        public void PrintItemListToConsole()
        {
            IEnumerable<T> z = GetAllAsync().Result;

            foreach(var x in z)
            {
                Console.WriteLine(x.ToString());
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                 var rez= await _context.Set<T>().ToListAsync();

                return rez;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<T>();  
            }
            
        }

        public void PrintItemListToFile(string path)
        {
            FileWriter file = new FileWriter(path);
            int counter = 0;

            /*
            _dbSet.ForEachAsync(x => {
                file.DoWrite(x.ToString());
                counter++;
            });
            */

            IEnumerable<T> z = GetAllAsync().Result;
            foreach (var x in z)
            {
                file.DoWrite(x.ToString());
                counter++;
            }
            file.DoWrite($"Обработано {counter} записей");
            file.Close();
        }

        public void SaveChanges()
        {
            Console.WriteLine($"Saving changes");
            _context.SaveChanges();
        }

      /*
        public void Dispose()
        {
            _context.Dispose();
        }
      */

        public void PrepareDb()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }


    }
}