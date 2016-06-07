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
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ok_password_ko_time()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ko_password_ok_time()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ko_password_ko_time()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ko_user_ok_pass_and_time()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ok_user_ko_pass_and_time()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Test_check_otp_ko_user_ko_pass_and_time()
        {
            Assert.IsTrue(false);
        }
    }
}
