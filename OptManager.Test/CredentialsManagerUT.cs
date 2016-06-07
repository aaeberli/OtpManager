using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OtpManager.Test
{
    using Application;
    using Application.Abstract;
    using Common;
    using Common.Abstract;
    using DataAccess;
    using DataAccess.Repositories;
    using Domain.Model;
    using Microsoft.Practices.Unity;
    using System.Linq;

    using static Utils;
    using static System.Threading.Thread;

    [TestClass]
    public class CredentialsManagerUT
    {
        private UnityContainer container;
        private ICredentialsManager<ApplicationRule> application;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IRepository<User>, UserRepository>()
                .RegisterType<IOtpGenerator, FakeOtpGenerator>()
                .RegisterType<ILogger, FakeLogger>()
                .RegisterType<ICredentialsManager<ApplicationRule>, CredentialsManager>();

            application = container.Resolve<ICredentialsManager<ApplicationRule>>();
        }

        [TestMethod]
        public void Test_create_otp_new()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            string test_user = "test_user";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);

            // Assert
            Assert.IsNotNull(password);
            Assert.IsTrue(koRules.Count() == 0);
        }


        [TestMethod]
        public void Test_create_otp_new_with_spaces()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            string test_user = "test user";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var validationRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementValidation && !r.Result);

            // Assert
            Assert.IsNull(password);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(validationRule);
        }


        [TestMethod]
        public void Test_create_otp_with_userid_too_long()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            // userid length > 50
            string test_user = "1234567890_1234567890_1234567890_1234567890_1234567890_1234567890";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var lengthRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.UserIdLength && !r.Result);

            // Assert
            Assert.IsNull(password);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(lengthRule);
        }

        [TestMethod]
        public void Test_create_otp_with_userid_empty()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            // userid length > 50
            string test_user = "";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var validationRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementValidation && !r.Result);

            // Assert
            Assert.IsNull(password);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(validationRule);
        }

        [TestMethod]
        public void Test_create_otp_with_otp_already_present()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            string test_user = "test_user";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            application.ResetRules();

            Sleep(15000);
            string password2 = application.CreateOtp(test_user);
            var koRules2 = application.ApplicationRules.Where(r => !r.Result);
            var duplicationRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementDuplication && !r.Result);

            // Assert
            Assert.IsNull(password2);
            Assert.IsTrue(koRules2.Count() > 0);
            Assert.IsNotNull(duplicationRule);
        }

        [TestMethod]
        public void Test_create_otp_with_otp_already_present_case_insensitiveness()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            string test_user = "test_user";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            application.ResetRules();

            Sleep(5000);
            string password2 = application.CreateOtp(test_user.ToUpper());
            var koRules2 = application.ApplicationRules.Where(r => !r.Result);
            var duplicationRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementDuplication && !r.Result);

            // Assert
            Assert.IsNull(password2);
            Assert.IsTrue(koRules2.Count() > 0);
            Assert.IsNotNull(duplicationRule);
        }

        [TestMethod]
        public void Test_create_otp_with_otp_already_present_and_expired()
        {
            // Arrange
            CleanTables();
            application.ResetRules();
            string test_user = "test_user";

            // Act
            string password = application.CreateOtp(test_user);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            application.ResetRules();

            Sleep(30000);
            string password2 = application.CreateOtp(test_user);
            var koRules2 = application.ApplicationRules.Where(r => !r.Result);
            var duplicationRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementDuplication && !r.Result);

            // Assert
            Assert.IsNotNull(password2);
            Assert.AreNotEqual(password, password2);
            Assert.IsTrue(koRules2.Count() == 0);
        }

        [TestMethod]
        public void Test_check_otp_ok_password_and_time()
        {
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(test_user, test_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);

            // Assert
            Assert.IsTrue(check);
            Assert.IsTrue(koRules.Count() == 0);
        }

        [TestMethod]
        public void Test_check_otp_too_long_user_ok_password_and_time()
        {
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            string too_long_user = "1234567890_1234567890_1234567890_1234567890_1234567890_1234567890";

            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(too_long_user, test_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var lengthRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.UserIdLength && !r.Result);
            var wrongUserRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.WrongUser && !r.Result);

            // Assert
            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(lengthRule);
            Assert.IsNotNull(wrongUserRule);
        }

        [TestMethod]
        public void Test_check_otp_ok_password_and_time_case_insensitiveness()
        {
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(test_user.ToUpper(), test_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var lengthRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.UserIdLength && !r.Result);

            // Assert
            Assert.IsTrue(check);
            Assert.IsTrue(koRules.Count() == 0);
        }

        [TestMethod]
        public void Test_check_otp_ok_password_ko_time()
        {
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(30000);
            bool check = application.CheckOtp(test_user, test_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var expiredRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementExpired && !r.Result);

            // Assert
            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(expiredRule);
        }

        [TestMethod]
        public void Test_check_otp_ko_password_ok_time()
        {
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            string wrong_pass = "wrong_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(test_user, wrong_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var wrongPassRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.WrongPassword && !r.Result);

            // Assert
            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(wrongPassRule);
        }

        [TestMethod]
        public void Test_check_otp_ko_password_ko_time()
        { 
            // Arrange
            string test_user = "test_user";
            string test_pass = "test_pass";
            string wrong_pass = "wrong_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(35000);
            bool check = application.CheckOtp(test_user, wrong_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var wrongPassRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.WrongPassword && !r.Result);
            var expiredRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.ElementExpired && !r.Result);

            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(wrongPassRule);
            Assert.IsNotNull(expiredRule);
        }

        [TestMethod]
        public void Test_check_otp_ko_user_ok_pass_and_time()
        {
            // Arrange
            string test_user = "test_user";
            string wrong_user = "wrong_user";
            string test_pass = "test_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(wrong_user, test_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var wrongUserRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.WrongUser && !r.Result);

            // Assert
            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(wrongUserRule);
        }

        [TestMethod]
        public void Test_check_otp_ko_user_ko_pass_and_time()
        {
            // Arrange
            string test_user = "test_user";
            string wrong_user = "wrong_user";
            string test_pass = "test_pass";
            string wrong_pass = "wrong_pass";
            CleanTables();
            CreateOtp(CreateUser(test_user), test_pass, DateTime.Now);
            application.ResetRules();

            // Act
            Sleep(5000);
            bool check = application.CheckOtp(wrong_user, wrong_pass);
            var koRules = application.ApplicationRules.Where(r => !r.Result);
            var wrongUserRule = application.ApplicationRules.SingleOrDefault(r => r.Reason == ReasonEnum.WrongUser && !r.Result);

            // Assert
            Assert.IsFalse(check);
            Assert.IsTrue(koRules.Count() > 0);
            Assert.IsNotNull(wrongUserRule);
        }
    }
}
