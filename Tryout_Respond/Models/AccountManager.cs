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

        public bool LogIn(string username, string password)
        {
            List<object[]> result = connection.RunQuery("SELECT * FROM Accounts WHERE username = '" + username + "' AND password = '" + password + "'");

            if (result.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}