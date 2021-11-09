using Otus.Teaching.Concurrency.Import.Handler.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ServiceStack.Text;
using Otus.Teaching.Concurrency.Import.Core.Entities;

namespace Otus.Teaching.Concurrency.Import.DataGenerator.Generators
{
   public class CsvGenerator : IDataGenerator
    {
            private readonly string _fileName;

            private readonly int _dataCount;

            public CsvGenerator(string fileName, int dataCount)
            {
                _fileName = fileName;
                _dataCount = dataCount;
            }

            public void Generate()
            {
                Console.WriteLine($"Generating records...");

                var customers = RandomCustomerGenerator.Generate(_dataCount);

                using var stream = File.CreateText(_fileName);

                CsvSerializer.SerializeToWriter<List<Customer>>(customers, stream);
            
                stream.Close();

                Console.WriteLine($"Successfully generated {_dataCount} records");

                Console.WriteLine("");
            }
        }
}
