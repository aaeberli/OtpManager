using System;
using OtpManager.Common.Abstract;

namespace OtpManager.Test
{
    internal class FakeOtpGenerator : IOtpGenerator
    {
        public string Create()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return $"test_password_{rnd.Next(100)}_{rnd.Next(100)}";
        }
    }
}