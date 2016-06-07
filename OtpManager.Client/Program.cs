using Microsoft.Practices.Unity;
using OtpManager.Application;
using OtpManager.Application.Abstract;
using OtpManager.Common;
using OtpManager.Common.Abstract;
using OtpManager.DataAccess;
using OtpManager.DataAccess.Repositories;
using OtpManager.Domain;
using OtpManager.Domain.Model;
using System;
using System.Linq;

namespace OtpManager.Client
{
    class Program
    {
        private static UnityContainer container;
        private static ICredentialsManager<ApplicationRule> application;

        static void Main(string[] args)
        {
            Init();
            Console.Write("Insert your username to get your password: ");
            string userid = Console.ReadLine();
            string otp = application.CreateOtp(userid);
            if (otp == null)
            {
                Console.WriteLine("Something went wrong!");
                Console.WriteLine("Reasons:");
                foreach (var rule in application.ApplicationRules.Where(r => !r.Result))
                {
                    Console.WriteLine($" - {rule.Reason.ToString()}");
                }
            }
            else
            {
                Console.WriteLine($"Your password is: {otp}");

                Console.Write("Insert you username: ");
                string checkingUserid = Console.ReadLine();
                Console.Write("Please type your password in 30 seconds: ");
                string checkingOtp = Console.ReadLine();
                bool granted = application.CheckOtp(checkingUserid, checkingOtp);
                if (granted)
                    Console.WriteLine("Your access is granted!");
                else
                {
                    Console.WriteLine("Access is denied!");
                    Console.WriteLine("Reasons:");
                    foreach (var rule in application.ApplicationRules.Where(r => !r.Result))
                    {
                        Console.WriteLine($" - {rule.Reason.ToString()}");
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void Init()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IRepository<User>, UserRepository>()
                .RegisterType<IOtpGenerator, OtpGenerator>()
                .RegisterType<ILogger, ConsoleLogger>()
                .RegisterType<ICredentialsManager<ApplicationRule>, CredentialsManager>();

            application = container.Resolve<ICredentialsManager<ApplicationRule>>();
        }
    }
}
