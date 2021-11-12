using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.Loader;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Otus.Teaching.Concurrency.Import.Core.Repositories;

namespace Otus.Teaching.Concurrency.Import.Core.Loaders
{
    public class DataLoader : IDataLoader
    {
        private readonly int _threadCount;

        private readonly List<Customer> _customers;
        public DataLoader(List<Customer> customers, int threadsCount)
        {
            _threadCount = threadsCount;
            _customers = customers;
        }

        public void LoadData()
        {
            var partSize = _customers.Count / _threadCount;

            var stopEvents = new WaitHandle[_threadCount];

            var sw = new Stopwatch();

            Console.WriteLine($"Starting data load with {_threadCount} threads");

            sw.Start();

            for (int i = 0; i < _threadCount; i++)
            {

                var customersQuery = _customers.Skip(partSize * i);

                var customersPart = i < _threadCount - 1
                    ? customersQuery.Take(partSize).ToList()
                    : customersQuery.ToList();

                var autoResetEvent = new AutoResetEvent(false);

                MyTreadClass m = new MyTreadClass(i, customersPart, autoResetEvent);
                //ThreadPool.QueueUserWorkItem(x => LoadTread(customersPart, autoResetEvent, i));
                stopEvents[i] = autoResetEvent;

            }

            ConsoleWriter.WriteDefault("All treads started");

            WaitHandle.WaitAll(stopEvents);

            sw.Stop();

            Console.WriteLine("Finished loading data");

            Console.WriteLine($"Total elapsed time: {sw.Elapsed.TotalSeconds:F}");
        }

        public class MyTreadClass
        {
            //сделан для того, чтобы каждый тред был инкапсулирован своим потоком
            Thread _myThread;

            public MyTreadClass (int i, List<Customer> customersPart, AutoResetEvent autoResetEvent)
            {
                bool success = false;
                do
                {
                    try
                    {
                        _myThread = new Thread(x => LoadTread(customersPart, autoResetEvent, i));
                        _myThread.Name = "trName" + i.ToString();
                        _myThread.Start();
                        success = true;
                    }
                    catch
                    {
                    }
                }
                while (!success);
            }

            private void LoadTread(List<Customer> customers, AutoResetEvent autoResetEvent, int threadNo)
            {
                ConsoleWriter.WriteDefault($"Thread №={threadNo} ManagedThreadId={Thread.CurrentThread.ManagedThreadId} trName = {Thread.CurrentThread.Name} started saving {customers.Count} objects");
                var sw = new Stopwatch();
                sw.Start();
                using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>(Program.context))
                {
                    foreach (Customer customer in customers)
                    {
                        //цикл сохранения для numberOfTries попыток
                        try
                        {
                            for (int i = 0; i < Program.numberOfTries; i++)
                            {
                                repository.AddItem(customer);
                                break;
                            }
                        }
                        catch
                        {
                            //ждем, пока освободится
                            Thread.Sleep(Program.threadWaitSleepMs);
                        }
                    }
                }
                
                sw.Stop();
                
                double totalElapsedTime = sw.Elapsed.TotalSeconds;
                
                double totalRecordCount = customers.Count;

                double avgTimePerRecord = Math.Round((totalElapsedTime / totalRecordCount), 5);

                ConsoleWriter.WriteDefault($"Thread №={threadNo} ManagedThreadId={Thread.CurrentThread.ManagedThreadId} finished, elapsed time: {totalElapsedTime} ms, averageTimePerRecord: {avgTimePerRecord} ms");
                
                autoResetEvent.Set();
            }
        }
    }
}