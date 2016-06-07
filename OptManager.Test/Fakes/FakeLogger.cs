using System;
using OtpManager.Common.Abstract;
using System.Diagnostics;

namespace OtpManager.Test
{
    internal class FakeLogger : ILogger
    {
        public void LogError(Exception ex)
        {
            Debug.Write(ex.Message);
        }

        public void LogInfo(string message)
        {
            Debug.Write(message);
        }
    }
}