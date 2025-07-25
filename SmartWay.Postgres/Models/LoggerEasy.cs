using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartWay.Postgres.Interfaces;
using System.Threading.Tasks;

namespace SmartWay.Postgres.Models
{
    public class LoggerEasy : ILoggerEasy
    {
        public void Info(string message)
        {
            Console.WriteLine($"Status: Info\t{message}");
        }

        public void Debug(string message)
        {
            Console.WriteLine($"Status: Debug\t{message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"Status: Error\t{message}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"Status: Warn\t{message}");
        }
    }
}
