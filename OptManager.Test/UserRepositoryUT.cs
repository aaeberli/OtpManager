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
            string test_user = "test_user";
            CreateOtp(CreateUser(test_user));

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
            string test_user = "test_user";
            CreateOtp(CreateUser(test_user));

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

        [TestMethod]
        public void Test_read_user_and_otp()
        {
            // Arrange
            CleanTables();
            string test_user1 = "test_user1";
            string test_user2 = "test_user2";
            string password1 = "test_pass1";
            string password2 = "test_pass2";
            CreateOtp(CreateUser(test_user1), password1);
            CreateOtp(CreateUser(test_user2), password2);

            // Act
            ICollection<User> users = repo.Read();
            IEnumerable<Otp> otps = repo.Read().Select(u => u.Otp);
            // Assert
            Assert.IsNotNull(users.Count == 2);
            Assert.IsNotNull(otps.Count() == 2);
        }

        [TestMethod]
        public void Test_read_user_and_otp_with_filter()
        {
            // Arrange
            CleanTables();
            string test_user1 = "test_user1";
            string test_user2 = "test_user2";
            string password1 = "test_pass1";
            string password2 = "test_pass2";
            CreateOtp(CreateUser(test_user1), password1);
            CreateOtp(CreateUser(test_user2), password2);

            // Act
            IEnumerable<User> users = repo.Read(u => u.UserId == test_user1);
            IEnumerable<Otp> otps = repo.Read().Select(u => u.Otp);
            // Assert
            Assert.IsNotNull(users.Count() == 1);
            Assert.IsNotNull(otps.Count() == 1);
        }

        [TestMethod]
        public void Test_delete_multimple_users_and_opts()
        {
            // Arrange
            CleanTables();
            string test_user1 = "test_user1";
            string test_user2 = "test_user2";
            string password1 = "test_pass1";
            string password2 = "test_pass2";
            CreateOtp(CreateUser(test_user1), password1);
            CreateOtp(CreateUser(test_user2), password2);

            // Act
            IEnumerable<User> users = repo.Read(u => u.UserId == test_user1 || u.UserId == test_user2);
            IEnumerable<Otp> otps = users.Select(u => u.Otp);
            foreach (User user in users)
            {
                user.Otp.User = null;
            }
            repo.Remove(users);
            repo.SaveChanges();
            DataTable otpTable = QueryTable("SELECT * FROM [Otp]");

            // Assert
            Assert.IsNotNull(users);
            Assert.IsTrue(repo.Read().Count == 0);
            Assert.IsTrue(otpTable.Rows.Count == 0);
        }

        [TestMethod]
        public void Test_pick_single_element_not_null(Expression<Func<User, bool>> filter)
        {
            // Arrange
            CleanTables();
            string test_user1 = "test_user1";
            string test_user2 = "test_user2";
            string password1 = "test_pass1";
            string password2 = "test_pass2";
            CreateOtp(CreateUser(test_user1), password1);
            CreateOtp(CreateUser(test_user2), password2);

            // Act
            User user = repo.SingleOrDefault(u => u.UserId == test_user2);

            // Assert
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Otp);
        }

        [TestMethod]
        public void Test_pick_single_element_null(Expression<Func<User, bool>> filter)
        {
            // Arrange
            CleanTables();
            string test_user1 = "test_user1";
            string test_user2 = "test_user2";
            string password1 = "test_pass1";
            string password2 = "test_pass2";
            CreateOtp(CreateUser(test_user1), password1);
            CreateOtp(CreateUser(test_user2), password2);
            string non_existent_user = "non_existent_user";

            // Act
            User user = repo.SingleOrDefault(u => u.UserId == non_existent_user);

            // Assert
            Assert.IsNull(user);
        }
    }
}
