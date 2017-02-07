using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Tryout_Respond.Models
{
    public class AccountManager
    {
        DatabaseConnection connection;

        public AccountManager()
        {
            connection = new DatabaseConnection();
        }

        public string Authenticate(string username, string password)
        {
            List<object[]> result = connection.RunQuery("SELECT * FROM Users WHERE username = '" + username + "' AND password = '" + password + "'");

            int userID = -1;

            string token = "";

            foreach (object[] value in result)
            {
                token = value[1].ToString() + "|" + DateTime.UtcNow.ToString();
            }

            return token;
        }
    }
}