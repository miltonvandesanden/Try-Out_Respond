using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tryout_Respond.Models;

namespace Tryout_Respond
{
    public class DatabaseConnection
    {
        public SqlConnection sqlConnection { get; set; }
        public static string CONNECTIONSTRING { get; set; }

        public DatabaseConnection()
        {
            CONNECTIONSTRING = "Data Source=(local);Initial Catalog=Respond_TryOut_Database;Integrated Security=SSPI;";
            sqlConnection = new SqlConnection(CONNECTIONSTRING);
        }

        public bool RunNonQuery(SqlCommand sqlCommand)
        {
            var success = false;

            SqlTransaction transaction;

            transaction = sqlConnection.BeginTransaction();

            sqlCommand.Connection = sqlConnection;
            sqlCommand.Transaction = transaction;

            try
            {
                if (sqlCommand.ExecuteNonQuery() > 0)
                {
                    success = true;
                }

                transaction.Commit();
                Console.WriteLine("query: " + sqlCommand.ToString() + "succesfully executed");

                return success;
            }
            catch (Exception queryExecutionException)
            {
                Console.WriteLine(queryExecutionException.InnerException);
                Console.WriteLine("failed to execute query: " + sqlCommand.ToString());

                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackException)
                {
                    Console.WriteLine("failed to rollback");
                }

                return success = false;
            }
        }

        public IList<object[]> RunQuery(SqlCommand sqlCommand)
        {
            SqlTransaction transaction;

            transaction = sqlConnection.BeginTransaction();

            sqlCommand.Connection = sqlConnection;
            sqlCommand.Transaction = transaction;

            var results = new List<object[]>();

            try
            {
                //command.CommandText = query;
                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    var row = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }

                    results.Add(row);
                }

                reader.Close();

                transaction.Commit();
                Console.WriteLine("query: " + sqlCommand.ToString() + "succesfully executed");

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to execute query: " + sqlCommand.ToString());

                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("failed to rollback");
                }

                return results = new List<object[]>();
            }
        }

        public bool SetToken(string token, DateTime expirationDate, string username)
        {
            var successfull = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET token=@token, token_expirationDate=@expirationDate WHERE username=@username", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@token", token);
                sqlCommand.Parameters["@token"].DbType = DbType.String;
                sqlCommand.Parameters["@token"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@expirationDate", expirationDate);
                sqlCommand.Parameters["@expirationDate"].DbType = DbType.DateTime;
                sqlCommand.Parameters["@expirationDate"].Size = 8;
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters["@username"].DbType = DbType.String;
                sqlCommand.Parameters["@username"].Size = 1073741823;
                sqlCommand.Prepare();

                successfull = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return successfull;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch(Exception exception2)
                {

                }

                return successfull = false;
            }
        }

        public bool RefreshToken(string oldToken, string newToken, DateTime expirationDate)
        {
            var successfull = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET token=@newToken, token_expirationDate=@expirationDate WHERE token=@oldToken", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@newToken", newToken);
                sqlCommand.Parameters["@newToken"].DbType = DbType.String;
                sqlCommand.Parameters["@newToken"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@expirationDate", expirationDate);
                sqlCommand.Parameters["@expirationDate"].DbType = DbType.DateTime;
                sqlCommand.Parameters["@expirationDate"].Size = 8;
                sqlCommand.Parameters.AddWithValue("@oldToken", oldToken);
                sqlCommand.Parameters["@oldToken"].DbType = DbType.String;
                sqlCommand.Parameters["@oldToken"].Size = 1073741823;
                sqlCommand.Prepare();

                successfull = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return successfull;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return successfull = false;
            }
        }

        public bool InsertUser(string userID, string username, string passwordHash, bool isAdmin)
        {
            var successfull = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("INSERT INTO Users(userID, username, passwordHash,isAdmin) VALUES(@userID, @username, @passwordHash, @isAdmin)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Parameters["@userID"].DbType = DbType.String;
                sqlCommand.Parameters["@userID"].Size = 6;
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters["@username"].DbType = DbType.String;
                sqlCommand.Parameters["@username"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                sqlCommand.Parameters["@passwordHash"].DbType = DbType.String;
                sqlCommand.Parameters["@passwordHash"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@isAdmin", isAdmin);
                sqlCommand.Parameters["@isAdmin"].DbType = DbType.Boolean;
                sqlCommand.Parameters["@isAdmin"].Size = 1;
                sqlCommand.Prepare();

                successfull = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return successfull;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return successfull = false;
            }
        }

        public bool IsTokenValid(string token)
        {
            var isValid = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT token FROM Users WHERE token=@token AND token_ExpirationDate>@currentDate", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@token", token);
                sqlCommand.Parameters["@token"].DbType = DbType.String;
                sqlCommand.Parameters["@token"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@currentDate", DateTime.UtcNow/*.ToString("yyyyMMdd HH:mm")*/);
                sqlCommand.Parameters["@currentDate"].DbType = DbType.DateTime;
                sqlCommand.Parameters["@currentDate"].Size = 8;
                sqlCommand.Prepare();

                isValid = RunQuery(sqlCommand).Any();

                sqlConnection.Close();

                return isValid;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return isValid = false;
            }
        }

        public bool DeleteToken(string token)
        {
            var success = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET token=null, token_expirationDate=null WHERE token=@token", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@token", token);
                sqlCommand.Parameters["@token"].DbType = DbType.String;
                sqlCommand.Parameters["@token"].Size = 1073741823;
                sqlCommand.Prepare();

                success = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return success;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return success = false;
            }
        }

        public bool ChangePassword(string userID, string passwordHash)
        {
            var success = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET passwordHash=@passwordHash WHERE userID=@userID", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                sqlCommand.Parameters["@passwordHash"].DbType = DbType.String;
                sqlCommand.Parameters["@passwordHash"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Parameters["@userID"].DbType = DbType.String;
                sqlCommand.Parameters["@userID"].Size = 6;
                sqlCommand.Prepare();

                success = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return success;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return success = false;
            }
        }

        public bool MakeAdmin(string userID, bool isAdmin)
        {
            var success = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET isAdmin=@isAdmin WHERE userID=@userID");
                sqlCommand.Parameters.AddWithValue("@isAdmin", isAdmin);
                sqlCommand.Parameters["@isAdmin"].DbType = DbType.Boolean;
                sqlCommand.Parameters["@isAdmin"].Size = 1;
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Parameters["@userID"].DbType = DbType.String;
                sqlCommand.Parameters["@userID"].Size = 6;

                success = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return success;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return success = false;
            }
        }

        public IList<User> GetUsers()
        {
            IList<User> users = new List<User>();

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT userID, username, isAdmin FROM Users", sqlConnection);
                sqlCommand.Prepare();

                //accounts = RunQuery(sqlCommand);

                foreach(object[] result in RunQuery(sqlCommand))
                {
                    User user = new User();
                    user.userID = result[0].ToString();
                    user.username = result[1].ToString();
                    user.isAdmin = (bool) result[2];

                    users.Add(user);
                }

                sqlConnection.Close();

                return users;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return users = new List<User>();
            }
        }

        public bool SetUsername(string token, string username)
        {
            var success = false;

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("UPDATE Users SET username WHERE token=@token", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@token", token);
                sqlCommand.Parameters["@token"].DbType = DbType.String;
                sqlCommand.Parameters["@token"].Size = 1073741823;
                sqlCommand.Prepare();

                success = RunNonQuery(sqlCommand);

                sqlConnection.Close();

                return success;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return success = false;
            }
        }

        public User GetUser(string username)
        {
            User user = new User();

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT userID, username, isAdmin FROM Users WHERE username=@username", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters["@username"].DbType = DbType.String;
                sqlCommand.Parameters["@username"].Size = 1073741823;
                sqlCommand.Prepare();

                //success = RunNonQuery(sqlCommand);
                object[] result = RunQuery(sqlCommand).First();

                sqlConnection.Close();

                user.userID = result[0].ToString();
                user.username = result[1].ToString();
                user.isAdmin = (bool) result[2];

                return user;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return user = null;
            }
        }

        public User GetUserByUserID(string userID)
        {
            User user = new User();

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT userID, username, isAdmin FROM Users WHERE userID=@userID", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Parameters["@userID"].DbType = DbType.String;
                sqlCommand.Parameters["@userID"].Size = 6;
                sqlCommand.Prepare();

                object[] result = RunQuery(sqlCommand).First();

                sqlConnection.Close();

                user.userID = result[0].ToString();
                user.username = result[1].ToString();
                user.isAdmin = (bool)result[2];

                return user;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return user = null;
            }
        }

        public User GetUserByUserIDWithToken(string userID)
        {
            User user = new User();

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT userID, username, isAdmin, token FROM Users WHERE userID=@userID", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userID", userID);
                sqlCommand.Parameters["@userID"].DbType = DbType.String;
                sqlCommand.Parameters["@userID"].Size = 6;
                sqlCommand.Prepare();

                object[] result = RunQuery(sqlCommand).First();

                sqlConnection.Close();

                user.userID = result[0].ToString();
                user.username = result[1].ToString();
                user.isAdmin = (bool)result[2];
                user.token = result[3].ToString();

                return user;
            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return user = null;
            }
        }


        public User GetUser(string username, string passwordHash)
        {
            User user = new User();

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT userID, username, isAdmin FROM Users WHERE username=@username AND passwordHash=@passwordHash", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters["@username"].DbType = DbType.String;
                sqlCommand.Parameters["@username"].Size = 1073741823;
                sqlCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                sqlCommand.Parameters["@passwordHash"].DbType = DbType.String;
                sqlCommand.Parameters["@passwordHash"].Size = 1073741823;
                sqlCommand.Prepare();

                //success = RunNonQuery(sqlCommand);
                object[] result = RunQuery(sqlCommand).FirstOrDefault();

                sqlConnection.Close();

                user.userID = result[0].ToString();
                user.username = result[1].ToString();
                user.isAdmin = (bool)result[2];

                return user;

            }
            catch (Exception exception)
            {
                try
                {
                    sqlConnection.Close();
                }
                catch (Exception exception2)
                {

                }

                return user = null;
            }
        }
    }
}