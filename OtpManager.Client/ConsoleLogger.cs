using OtpManager.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Client
{
    internal class ConsoleLogger : ILogger
    {
        public void LogError(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }
    }
}
