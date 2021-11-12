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
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Loader
{
   public  class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.csv");
        private static string _printFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customersDump.txt");
        private static string _postErrorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.html");
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


            var client = new System.Net.Http.HttpClient(new HttpClientHandler());
            client.BaseAddress = new Uri(WebApiAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
                            string strPostRez = "";
                            Console.WriteLine($"Введите сообщение");
                            var s = Console.ReadLine();

                            ConsoleToApiMessage msg = new ConsoleToApiMessage()
                            {
                                Text = "This is console message: " + s,
                                TimeStamp = DateTime.Now
                            };


                        var json = JsonConvert.SerializeObject(msg, Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                        var response6 = client.PostAsync($"", jsonContent);

                        strPostRez = response6.Result.Content.ReadAsStringAsync().Result;
                        Console.WriteLine($"Server reply: code={response6.Result.StatusCode} ");
                        Console.WriteLine($"Server reply: {strPostRez} ");

                        break;



                    case 7:
                        //тестовый get-запрос
                           
                            var response7 = client.GetAsync("").Result;

                            Console.WriteLine($"Server reply: code={response7.StatusCode} ");
                            Console.WriteLine($"Server reply: {response7.Content.ReadAsStringAsync().Result}");
                            
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