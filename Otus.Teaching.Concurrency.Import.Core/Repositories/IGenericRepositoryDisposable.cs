using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.Repositories
{
    public interface IGenericRepositoryDisposable<T> : IGenericRepository<T>, IDisposable
    {
    
    }
}
