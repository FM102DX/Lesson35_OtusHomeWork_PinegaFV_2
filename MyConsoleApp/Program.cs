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
using System.Threading.Tasks;

namespace MyConsoleApp
{
    public class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.csv");
        private static string _printFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customersDump.txt");
        private static string _postErrorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.html");
        private static string _generatorAppLaunchPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Otus.Teaching.Concurrency.Import.DataGenerator.App.exe");
        public static int numberOfTries = 500; //количество попыток сохранения в БД
        public static int threadWaitSleepMs = 200;
        public static OtusMultiTreadDbContext context;

         // private const string WebApiAddress = "https://localhost:44339/";
        private const string WebApiAddress = "https://api.ricompany.info/";

        private static string token;

        

        static void Main(string[] args)
        {
            CommonOperationResult rez;
            IGenericRepository<ConsoleToApiMessage> messagesWebRepository = new GenericWebApiRepository<ConsoleToApiMessage>(WebApiAddress);
            IGenericRepository<Customer> customersWebRepository = new GenericWebApiRepository<Customer>(WebApiAddress+ "Customers/", true);

            int targetCustomerId = 0;
            bool canGoOut = false;
            Customer targetCustomer;

            do
            {
                int menuPoint = ShowMenu();
                switch (menuPoint)
                {
                    case 1:
                        //отправляем рандомный текст на сервер
                        Console.WriteLine($"Введите сообщение");
                        var s = Console.ReadLine();
                        ConsoleToApiMessage msg = new ConsoleToApiMessage()
                        {
                            Id=0,
                            Text = "Console message: " + s,
                            TimeStamp = DateTime.Now
                        };
                        rez= messagesWebRepository.AddItem(msg);

                        Console.WriteLine(rez.ToString());
                        break;

                    case 2:
                        //список клиентов
                        var customeList = customersWebRepository.GetAllItems();
                        
                        foreach (var c in customeList)
                        {
                            Console.WriteLine(c);
                        }

                        break;

                    case 3:
                        //информация по конкретному клиенту по id

                        do
                        {
                            Console.WriteLine("Введите номер клиента");
                            canGoOut = Int32.TryParse(Console.ReadLine(), out targetCustomerId);
                            if (!canGoOut) ConsoleWriter.WriteRed ("Введите корректный int");
                        }
                        while (!canGoOut);
                       
                        targetCustomer = customersWebRepository.GetItemByIdOrNull(targetCustomerId);

                        Console.WriteLine(targetCustomer?.ToString());

                        break;

                    case 4:
                        //сгенерировать клиента
                        Customer newCustomer = RandomCustomerGenerator.Generate(1)[0];
                        
                        newCustomer.Id = 0;

                        rez = customersWebRepository.AddItem(newCustomer);

                        Console.WriteLine(rez.ToString());

                        break;

                    case 5:
                        //сгенерировать клиента, который вызовет 409
                        Customer newCustomer2 = RandomCustomerGenerator.Generate(1)[0];

                        newCustomer2.Id = 1;

                        rez = customersWebRepository.AddItem(newCustomer2);

                        Console.WriteLine(rez.ToString());
                        break;

                    case 6:
                        //апедйт имени клиента
                        canGoOut = false;
                        do
                        {
                            Console.WriteLine("Введите номер клиента и фамилию через пробел, или 'exit'");

                            string[] updateStringArr = Console.ReadLine().Split(" ");

                            if (updateStringArr[0].ToLower() == "exit") break;

                            canGoOut = Int32.TryParse(updateStringArr[0], out targetCustomerId);
                            if (!canGoOut)
                            {
                                ConsoleWriter.WriteRed("Вы ввели некорректный номер клиента");
                                continue;
                            }

                            targetCustomer = customersWebRepository.GetItemByIdOrNull(targetCustomerId);
                            canGoOut = (targetCustomer != null);
                            if (!canGoOut)
                            {
                                ConsoleWriter.WriteRed("Клиента с таким нормером не существует");
                                continue;
                            }

                            targetCustomer.FullName = updateStringArr[1];
                            rez = customersWebRepository.UpdateItem(targetCustomer);
                            Console.WriteLine(rez.ToString());
                        }
                        while (!canGoOut);
                        break;

                    case 7:

                        do
                        {
                            Console.WriteLine("Введите номер клиента");
                            canGoOut = Int32.TryParse(Console.ReadLine(), out targetCustomerId);
                            if (!canGoOut)
                            {
                                ConsoleWriter.WriteRed("Введите корректный int");
                                continue;
                            }

                            targetCustomer = customersWebRepository.GetItemByIdOrNull(targetCustomerId);

                            canGoOut = (targetCustomer != null);

                            if (!canGoOut)
                            {
                                ConsoleWriter.WriteRed("Клиента с таким номером не существует");
                                continue;
                            }

                            rez = customersWebRepository.Delete(targetCustomer.Id);

                            Console.WriteLine(rez.ToString());

                        }
                        while (!canGoOut);

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
                Console.WriteLine("");
                ConsoleWriter.WriteGreen("Выберите одно из действий (введите число):");
                Console.WriteLine("1 - отправить сообщение на веб-консоль");
                Console.WriteLine("---------------------");
                Console.WriteLine("2 - запросить список клиентов");
                Console.WriteLine("3 - информация по конкретному клиенту по id");
                Console.WriteLine("4 - сгенерировать клиента");
                Console.WriteLine("5 - добавить клиента с id=1");
                Console.WriteLine("6 - апедйт имени клиента");
                Console.WriteLine("7 - удалить клиента");
                Console.WriteLine("---------------------");
                Console.WriteLine("9 - выход");
                Console.WriteLine("");

                string key = Console.ReadLine().Trim();

                if ((key == "1" || key == "2" || key == "3" || key == "4" || key == "5" || key == "6" || key == "7" || key == "8" || key == "9") && (Int32.TryParse(key, out rez)))
                {
                    correctInput = true;
                }
            }
            while (!correctInput);

            return rez;
        }

    }
}
