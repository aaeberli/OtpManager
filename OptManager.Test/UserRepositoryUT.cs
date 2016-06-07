using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using OtpManager.Common.Abstract;
using OtpManager.DataAccess;
using OtpManager.Domain.Model;
using OtpManager.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace OtpManager.Test
{
    using System.Data;
    using static Utils;

    [TestClass]
    public class UserRepositoryUT
    {
        private UnityContainer container;
        private IRepository<User> repo;

        [TestInitialize]
        public void TestInitialize()
        {
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IRepository<User>, UserRepository>();

            repo = container.Resolve<IRepository<User>>();
        }


        [TestMethod]
        public void Test_add_single_user_and_otp()
        {
            // Arrange
            CleanTables();
            User user = repo.Create();
            DateTime test_date = new DateTime(2016, 6, 7, 12, 13, 54);
            string test_pass = "test_pass";
            Otp otp = new Otp() { Password = test_pass, StartDate = test_date };
            string test_user = "test_user";
            user.UserId = test_user;
            user.Otp = otp;

            // Act
            repo.Add(user);
            repo.SaveChanges();
            DataTable userTable = QueryTable("SELECT * FROM [User]");
            DataTable otpTable = QueryTable("SELECT * FROM [Otp]");

            // Assert
            Assert.IsTrue(userTable.Rows.Count == 1);
            Assert.IsTrue(userTable.Rows[0]["UserId"].ToString() == test_user);
            Assert.IsTrue(otpTable.Rows[0]["Password"].ToString() == test_pass);
            Assert.AreEqual(Convert.ToDateTime(otpTable.Rows[0]["StartDate"]), test_date);
        }

        [TestMethod]
        public void Test_add_two_users_and_otps()
        {
            // Arrange
            CleanTables();
            User user = repo.Create();
            DateTime test_date = new DateTime(2016, 6, 7, 12, 13, 54);
            string test_pass = "test_pass";
            Otp otp = new Otp() { Password = test_pass, StartDate = test_date };
            string test_user = "test_user";
            user.UserId = test_user;
            user.Otp = otp;
            //------
            User user2 = repo.Create();
            DateTime test_date2 = new DateTime(2016, 6, 8, 12, 21, 12);
            string test_pass2 = "test_pass2";
            Otp otp2 = new Otp() { Password = test_pass2, StartDate = test_date2 };
            string test_user2 = "test_user2";
            user2.UserId = test_user2;
            user2.Otp = otp2;

            // Act
            repo.Add(new User[2] { user, user2 });
            repo.SaveChanges();
            DataTable userTable = QueryTable("SELECT * FROM [User]");
            DataTable otpTable = QueryTable("SELECT * FROM [Otp]");

            // Assert
            Assert.IsTrue(userTable.Rows.Count == 2);
            Assert.IsTrue(userTable.Rows[0]["UserId"].ToString() == test_user);
            Assert.IsTrue(otpTable.Rows[0]["Password"].ToString() == test_pass);
            Assert.AreEqual(Convert.ToDateTime(otpTable.Rows[0]["StartDate"]), test_date);
            //------
            Assert.IsTrue(userTable.Rows[1]["UserId"].ToString() == test_user2);
            Assert.IsTrue(otpTable.Rows[1]["Password"].ToString() == test_pass2);
            Assert.AreEqual(Convert.ToDateTime(otpTable.Rows[1]["StartDate"]), test_date2);
        }

        [TestMethod]
        public void Test_delete_single_users_and_otps()
        {
            // Arrange
            CleanTables();
            CreateOtp(CreateUser());
            string test_user = "test_user";

            // Act
            User user = repo.SingleOrDefault(u => u.UserId == test_user);
            if (user != null)
            {
                user.Otp.User = null;
                repo.Remove(user);
                repo.SaveChanges();
            }
            DataTable otpTable = QueryTable("SELECT * FROM [Otp]");

            // Assert
            Assert.IsNotNull(user);
            Assert.IsTrue(repo.Read().Count == 0);
            Assert.IsTrue(otpTable.Rows.Count == 0);
        }

        [TestMethod]
        public void Test_delete_single_users_and_otps_with_errors_then_add()
        {
            // Arrange
            string message = string.Empty;
            CleanTables();
            CreateOtp(CreateUser());
            string test_user = "test_user";

            // Act
            User user = repo.SingleOrDefault(u => u.UserId == test_user);
            if (user != null)
            {
                user.Otp.User = null;
                CleanTables();
                repo.Remove(user);
                try
                {
                    // errors on saveChanges should reset the state of the underlying DataContext
                    repo.SaveChanges();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            //---------
            user = repo.Create();
            DateTime test_date = new DateTime(2016, 6, 7, 12, 13, 54);
            string test_pass = "test_pass";
            Otp otp = new Otp() { Password = test_pass, StartDate = test_date };
            user.UserId = test_user;
            user.Otp = otp;
            repo.Add(user);
            repo.SaveChanges();
            DataTable userTable = QueryTable("SELECT * FROM [User]");
            DataTable otpTable = QueryTable("SELECT * FROM [Otp]");

            // Assert
            Assert.IsTrue(message != string.Empty);
            Assert.IsTrue(userTable.Rows.Count == 1);
            Assert.IsTrue(userTable.Rows[0]["UserId"].ToString() == test_user);
            Assert.IsTrue(otpTable.Rows[0]["Password"].ToString() == test_pass);
            Assert.AreEqual(Convert.ToDateTime(otpTable.Rows[0]["StartDate"]), test_date);
        }

        public User Add(User entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ICollection<User> Read()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Read(Expression<Func<User, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Remove(IEnumerable<User> entities)
        {
            throw new NotImplementedException();
        }

        public User Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public User SingleOrDefault(Expression<Func<User, bool>> filter)
        {
            throw new NotImplementedException();
        }


        public void UndoChanges()
        {
            throw new NotImplementedException();
        }
    }
}
