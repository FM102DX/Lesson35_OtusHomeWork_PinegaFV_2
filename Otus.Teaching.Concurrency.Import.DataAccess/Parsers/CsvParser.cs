using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using ServiceStack.Text;
using System.Collections.Generic;
using System.IO;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class CsvParser<T>: IDataParser<T>
    {
        private string _path;

        public CsvParser(string path)
        {
            _path = path;
        }

        public T Parse()
        {
            using var stream = File.OpenText(_path);
            var customers = CsvSerializer.DeserializeFromReader<T>(stream);
            return customers;
        }

    }
}
