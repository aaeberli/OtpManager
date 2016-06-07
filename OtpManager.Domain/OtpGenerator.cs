using OtpManager.Common;
using OtpManager.Common.Abstract;
using OtpManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Domain
{
    public class OtpGenerator : IOtpGenerator
    {
        public string Create()
        {
            byte[] randomBytes = Guid.NewGuid().ToByteArray();
            List<byte> byteList = new List<byte>(randomBytes);
            byteList.FluentAdd(RndByteGenerator()).FluentAdd(RndByteGenerator());
            string randomString = Convert.ToBase64String(byteList.ToArray());
            return randomString;
        }


        private byte RndByteGenerator()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            byte[] buffer = new byte[1];
            rnd.NextBytes(buffer);
            return buffer[0];
        }
    }
}
