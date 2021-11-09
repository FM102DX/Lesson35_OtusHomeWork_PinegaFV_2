using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.DataAccess.Parsers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Otus.Teaching.Concurrency.Import.Loader
{
   public  class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.csv");
        private static string _printFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customersDump.txt");
        private static string _generatorAppLaunchPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Otus.Teaching.Concurrency.Import.DataGenerator.App.exe");
        private static int _treadCount = 1;
        private static int _recordsCount = 10;
        public static int numberOfTries = 500; //количество попыток сохранения в БД
        public static int threadWaitSleepMs = 200;
        public static OtusMultiTreadDbContext context;

        //  private const string WebApiAddress = "https://localhost:44318/";
        private const string WebApiAddress = "https://api.ricompany.info/";

        private static string token;

        static void Main(string[] args)
        {
            //создать dbContext
            context = new OtusMultiTreadDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            #region h1

            /*
             *             
             *             bool x;
            using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>(context))
            {
               try
                {
                   x= repository.AddItemAsync(new Customer() { FullName = "Name", Id = 3 }).Result;
                   x= repository.AddItemAsync(new Customer() { FullName = "Name123", Id = 1 }).Result;
                   repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error01: {ex.Message}");
                }
            }

            using (IGenericRepositoryDisposable<ConsoleToApiMessage> repository = new GenericRepositoryDisposable<ConsoleToApiMessage>(context))
            {
                repository.AddItem(new ConsoleToApiMessage() { Id = 1, Text = "Text1", TimeStamp = DateTime.Now });
                repository.AddItem(new ConsoleToApiMessage() { Id = 2, Text = "Text2", TimeStamp = DateTime.Now });
                repository.SaveChanges();
            }


            using (IGenericRepositoryDisposable<ConsoleToApiMessage> repository = new GenericRepositoryDisposable<ConsoleToApiMessage>())
            {
                repository.PrepareDb();
            }
            */
            /*
            using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>())
            {
                repository.AddItem(new Customer() { FullName = "Name", Id = 3 });
                repository.SaveChanges();
            }

            using (IGenericRepositoryDisposable<ConsoleToApiMessage> repository = new GenericRepositoryDisposable<ConsoleToApiMessage>())
            {
                repository.AddItem(new ConsoleToApiMessage() { Id = 1, Text = "Text1", TimeStamp = DateTime.Now });
                repository.AddItem(new ConsoleToApiMessage() { Id = 2, Text = "Text2", TimeStamp = DateTime.Now });
                repository.SaveChanges();
            }


            using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>())
            {
                List<Customer> customers = new List<Customer>();
                customers.Add(new Customer() { FullName = "Name", Id = 3 });
                customers.Add(new Customer() { FullName = "Name2", Id = 2 });

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
                    }
                }
                repository.SaveChanges();
            }
            */
            #endregion 

            do
            {
                int menuPoint = ShowMenu();
                switch (menuPoint)
                {
                    case 1:
                        //генерируем файл с клиентами (сериализация) запуском метода
                        var xmlGenerator = new CsvGenerator(_dataFilePath, _recordsCount);
                        xmlGenerator.Generate();
                        break;

                    case 2:
                        //генерируем файл с клиентами (сериализация) через отдельгный процесс
                        var startInfo = new ProcessStartInfo()
                        {
                            ArgumentList = { _dataFilePath, _recordsCount.ToString() },
                            FileName = _generatorAppLaunchPath
                        };
                        var processGenerator = Process.Start(startInfo);
                        Console.WriteLine($"Data generation started with process Id {processGenerator.Id}...");
                        processGenerator.WaitForExit();
                        break;

                    case 3:
                        //десериализация
                        var customers = new CsvParser<List<Customer>>(_dataFilePath).Parse();
                        Console.WriteLine($"Десериализация успешна, получено {customers.Count} объектов");

                        //обработка полученного массива объектов в несколько потоков
                        Console.WriteLine("");
                        Console.WriteLine($" Начинаем парсинг в {_treadCount} потоков");
                        var loader = new DataLoader(customers, _treadCount);
                        loader.LoadData();
                        break;

                    case 4:
                        //читаем из sqlite
                        using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>(context))
                        {
                            repository.PrintItemListToConsole();
                        }
                        break;

                    case 5:
                        //читаем из sqlite в файл
                        using (IGenericRepositoryDisposable<Customer> repository = new GenericRepositoryDisposable<Customer>(context))
                        {
                            repository.PrintItemListToFile(_printFilePath);
                        }
                        break;

                    case 6:
                        //отправляем рандомный текст на сервер
                        using (var client = new System.Net.Http.HttpClient())
                        {
                            Console.WriteLine($"Введите сообщение");
                            var s = Console.ReadLine();
                            client.BaseAddress = new Uri(WebApiAddress);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            ConsoleToApiMessage msg = new ConsoleToApiMessage()
                            {
                                Text = "This is console message: " + s,
                                TimeStamp = DateTime.Now
                            };

                            var response = client.PostAsJsonAsync<ConsoleToApiMessage>("", msg).Result;
                            Console.WriteLine($"Server reply: code={response.StatusCode} ");
                            Console.WriteLine($"Server reply: {response.Content.ReadAsStringAsync().Result} ");

                            
                            //var response2 = client.PostAsync("users/", user, new JsonMediaTypeFormatter());
                        }
                        break;



                    case 7:
                        //тестовый get-запрос
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(WebApiAddress);
                            client.DefaultRequestHeaders.Accept.Clear();
                            
                            var response = client.GetAsync("").Result;

                            Console.WriteLine($"Server reply: code={response.StatusCode} ");
                            Console.WriteLine($"Server reply: {response.Content.ReadAsStringAsync().Result}");
                            
                        }
                        break;

                    case 9:
                        return;

                    default:

                        break;
                }

            }
            while (true);
        }

        static int ShowMenu()
        {
            bool correctInput = false;
            int rez = 0;
            do
            {
                Console.WriteLine("");
                Console.WriteLine("Выберите одно из действий (введите число):");
                Console.WriteLine("1 - сегенрировать тестовые данные customers.csv (запуск метода)");
                Console.WriteLine("2 - сегенрировать тестовые данные customers.csv (запуск в виде отдельного процесса)");
                Console.WriteLine("---------------------");
                Console.WriteLine("3 - парсинг тестовых данных с записью в базу sqlite");
                Console.WriteLine("4 - чтение из базы sqlite в консоль");
                Console.WriteLine("5 - чтение из базы sqlite в файл");
                Console.WriteLine("---------------------");
                Console.WriteLine("6 - текстовое сообщение на сервер");
                Console.WriteLine("7 - тестовый GET запрос");
                Console.WriteLine("---------------------");
                Console.WriteLine("9 - выход");
                Console.WriteLine("");

                string key = Console.ReadLine().Trim();

                if ((key == "1" || key == "2" || key == "3" || key == "4" || key == "5" || key == "6" || key == "7" || key == "8" ||  key == "9") && (Int32.TryParse(key, out rez)))
                {
                    correctInput = true;
                }
            }
            while (!correctInput);

            return rez;
        }

    }
}