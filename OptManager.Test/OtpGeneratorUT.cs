using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OtpManager.Common.Abstract;
using OtpManager.Domain;
using Microsoft.Practices.Unity;

namespace OtpManager.Test
{
    [TestClass]
    public class OtpGeneratorUT
    {
        IOtpGenerator otpGenerator;
        UnityContainer container;

        [TestInitialize]
        public void TestInitialize()
        {
            container = new UnityContainer();

            container
                .RegisterType<IOtpGenerator, OtpGenerator>();

            otpGenerator = container.Resolve<IOtpGenerator>();
        }

        [TestMethod]
        public void Test_password_length()
        {
            // Arrange

            // Act
            string password = otpGenerator.Create();

            // Assert
            Assert.IsNotNull(password);
            Assert.IsTrue(password.Length == 24);
        }

        [TestMethod]
        public void Test_passwords_are_different()
        {
            // Arrange

            // Act
            string password1 = otpGenerator.Create();
            string password2 = otpGenerator.Create();

            // Assert
            Assert.IsNotNull(password1);
            Assert.IsNotNull(password2);
            Assert.IsTrue(password1.Length == 24);
            Assert.IsTrue(password2.Length == 24);
            Assert.AreNotEqual(password1, password2);
        }
    }
}
