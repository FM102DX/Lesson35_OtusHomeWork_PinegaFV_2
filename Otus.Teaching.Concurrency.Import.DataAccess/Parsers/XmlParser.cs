using System.Collections.Generic;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Core.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class XmlParser: IDataParser<List<Customer>>
    {
        private string _dataFilePath;
        public XmlParser(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public List<Customer> Parse()
        {
            //Parse data
            return new List<Customer>();
        }
    }
}