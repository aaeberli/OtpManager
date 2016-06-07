using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Test
{
    internal class Utils
    {
        public static void ExecuteNonQuery(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable QueryTable(string cmdText)
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// Cleans DB for tests
        /// </summary>
        public static void CleanTables()
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
        public static object CreateUser(string userId)
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;

            string cmdText_create_user = $"INSERT INTO [User] values ('{userId}'); SELECT @@IDENTITY;";
            return ExecuteScalar(connString, cmdText_create_user);
        }
        public static object CreateUser()
        {
            return CreateUser("test_user");
        }

        /// <summary>
        /// Creates an otp for the specified used directly in DB
        /// </summary>
        /// <returns></returns>
        public static void CreateOtp(object id, string password, DateTime? startDate = null)
        {
            string connString = ConfigurationManager.ConnectionStrings["OtpModel"].ConnectionString;

            string _startDate = startDate != null ? $"'{((DateTime)startDate).ToString("yyyy-MM-dd hh:mm:ss.fff tt")}'" : "GETDATE()";
            string cmdText_create_user = $"INSERT INTO [Otp] ([UserId],[Password],[StartDate]) values ({id},'{password}',{_startDate});";
            ExecuteNonQuery(connString, cmdText_create_user);
        }
        public static void CreateOtp(object id)
        {
            CreateOtp(id, "test_pass", null);
        }
    }
}
