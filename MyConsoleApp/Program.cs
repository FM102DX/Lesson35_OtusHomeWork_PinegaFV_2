using System;

namespace MyConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //это консольное приложение для доступа к REST API
            do
            {
                int menuPoint = ShowMenu();
                switch (menuPoint)
                {
                    case 1:
                        Console.WriteLine($"");
                        break;

                    case 2:
                        Console.WriteLine($"");
                        break;

                    case 3:
                        Console.WriteLine($"");
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
                Console.WriteLine("1 - вывести всех пользователей");
                Console.WriteLine("2 - запросить пользователя по ID");
                Console.WriteLine("3 - сгенерировать и сохранить случайного пользователя");
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
