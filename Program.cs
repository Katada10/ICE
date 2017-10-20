using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var p = new Parser();

            var lines = File.ReadAllLines("example.txt");
            var lst = lines.OfType<string>().ToList();

           
            foreach (var line in lst)
            {
                p.Parse(line);
            }

            Console.ReadLine();
        }
    }
}
