using System;
using System.IO;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var t = new Tokenizer();
            var p = new Parser();

            var lines = File.ReadAllLines("example.txt");

            foreach (var line in lines)
            {
                p.Parse(line);
            }

            Console.ReadLine();
        }
    }
}
