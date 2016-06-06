using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Practices.Unity;
using OtpManager.Common.Abstract;
using OtpManager.DataAccess;
using OtpManager.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace OtpManager.Test
{
    [TestClass]
    public class DataAccessAdapterUT
    {
        private UnityContainer container;

        private void ExecuteNonQuery(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private object ExecuteScalar(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Cleans DB for tests
        /// </summary>
        private void CleanTables()
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;

            string cmdText_clear_opt = "DELETE FROM [Otp]";
            string cmdText_clear_user = "DELETE FROM [User]";
            ExecuteNonQuery(connString, cmdText_clear_opt);
            ExecuteNonQuery(connString, cmdText_clear_user);
        }

        /// <summary>
        /// Creates a test user directly in DB
        /// </summary>
        private object CreateUser()
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;

            string cmdText_create_user = "INSERT INTO [User] values ('test_user'); SELECT @@IDENTITY;";
            return ExecuteScalar(connString, cmdText_create_user);
        }

        /// <summary>
        /// Creates an otp for the specified used directly in DB
        /// </summary>
        /// <returns></returns>
        private void CreateOtp(object id)
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;

            string cmdText_create_user = $"INSERT INTO [Otp] ([UserId],[Password],[StartDate]) values ({id},'test_pass',GETDATE());";
            ExecuteNonQuery(connString, cmdText_create_user);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>();
        }

        [TestMethod]
        public void Test_clean_db()
        {
            CleanTables();
        }

        [TestMethod]
        public void Test_create_user()
        {
            // Act
            object id = CreateUser();

            // Assert
            Assert.IsNotNull(id);
            Assert.IsTrue(Convert.ToInt32(id) > 0);
        }

        [TestMethod]
        public void Test_dataAdapter_create_user()
        {
            // Arrange
            CleanTables();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            User user = new User { UserId = "testUser" };

            // Act
            adapter.Add<User>(user);
            adapter.SaveChanges();
            List<User> userList = adapter.GetEntities<User>().ToList();

            // Assert
            Assert.IsTrue(userList.Count == 1);
        }

        [TestMethod]
        public void Test_dataAdapter_create_user_and_otp()
        {
            // Arrange
            CleanTables();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            User user = new User { UserId = "testUser" };
            Otp otp = new Otp { User = user, Password = "test_pass", StartDate = DateTime.Now };

            // Act
            adapter.Add<User>(user);
            adapter.Add<Otp>(otp);
            adapter.SaveChanges();
            List<User> userList = adapter.GetEntities<User>().ToList();
            User addedUser = userList.SingleOrDefault(u => u.Id == user.Id);

            // Assert
            Assert.IsNotNull(addedUser);
            Assert.IsNotNull(addedUser.Otp);
        }

        [TestMethod]
        public void Test_dataAdapter_read_user()
        {
            // Arrange
            CleanTables();
            CreateUser();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();

            // Act
            List<User> userList = adapter.GetEntities<User>().ToList();

            // Assert
            Assert.IsTrue(userList.Count == 1);
        }

        [TestMethod]
        public void Test_dataAdapter_update_user_and_otp()
        {
            // Arrange
            CleanTables();
            CreateOtp(CreateUser());
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            DateTime testDate = new DateTime(2016, 6, 5, 12, 51, 12);
            string updatedUserId = "_updated";

            // Act
            List<User> userList = adapter.GetEntities<User>().ToList();
            User user = userList.First();
            string old_userId = user.UserId;
            if (user != null) user.UserId += updatedUserId;
            if (user.Otp != null) user.Otp.StartDate = testDate;
            adapter.SaveChanges();
            User updatedUser = adapter.GetEntities<User>().SingleOrDefault(u => u.Id == user.Id);

            // Assert
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Otp);
            Assert.IsNotNull(updatedUser);
            Assert.IsNotNull(updatedUser.Otp);
            Assert.IsTrue(updatedUser.UserId == old_userId + updatedUserId);
            Assert.IsTrue(updatedUser.Otp.StartDate.Equals(testDate));
        }

        [TestMethod]
        public void Test_dataAdapter_delete_user_and_otp()
        {
            // Arrange
            CleanTables();
            CreateOtp(CreateUser());
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();

            // Act
            List<User> userList = adapter.GetEntities<User>().ToList();
            User user = userList.First();
            adapter.Remove(user.Otp);
            adapter.Remove(user);
            adapter.SaveChanges();
            List<User> users = adapter.GetEntities<User>().ToList();
            List<Otp> otps = adapter.GetEntities<Otp>().ToList();

            // Assert
            Assert.IsTrue(users.Count() == 0);
            Assert.IsTrue(otps.Count() == 0);
        }
    }
}
