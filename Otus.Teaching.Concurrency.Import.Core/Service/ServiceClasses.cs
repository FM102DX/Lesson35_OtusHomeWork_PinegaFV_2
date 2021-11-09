using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.Service
{

    public static class ServiceFunctions
    {
        public static string generateNBlockGUID(int n, int bockLength = 4)
        {
            //return "AAAA-AAAA-AAAA-AAAA-...";
            string[] myArray = new string[n];
            for (int i = 0; i < n; i++)
            {
                myArray[i] = randomString(bockLength).ToUpper();
            }
            return string.Join("-", myArray);
        }

        private static Random random = new Random();
        public static string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

    public static class ConsoleWriter
    {
        //класс, который пишет в консоль разными цветами и пр.
        private static readonly object _locker = new object();
        public static void WriteWithColor(string text, ConsoleColor color)
        {
            lock (_locker) //если не использовать локер, цвета смешиваются
            {
                ConsoleColor _color = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.WriteLine("");
                Console.ForegroundColor = _color;
            }
        }

        public static void WriteDefault(string text) { WriteWithColor(text, Console.ForegroundColor); }

        public static void WriteRed(string text) { WriteWithColor(text, ConsoleColor.Red); }

        public static void WriteGreen(string text) { WriteWithColor(text, ConsoleColor.Green); }

        public static void WriteYellow(string text) { WriteWithColor(text, ConsoleColor.Yellow); }

    }

    public class FileWriter
    {
        StreamWriter _file;
        
        public FileWriter(string path)
        {
            _file= File.CreateText(path);
        }

        public void DoWrite(string text)
        {
            _file.WriteLine(text);
        }

        public void Close()
        {
            _file.Flush();
            _file.Close();
        }

    }


}
