using System;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class GenericRepositoryDisposable<T> : GenericRepository<T>, IGenericRepositoryDisposable<T>, IDisposable where T : KeepableClass
    {
        public void Dispose()
        {
            
        }

        public GenericRepositoryDisposable(OtusMultiTreadDbContext context): base(context)
        {
            
        }


    }
}