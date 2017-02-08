using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Tryout_Respond.Models
{
    public class AccountManager
    {
        public const int MINIMALPASSWORDLENGTH = 8;
        public const int TOKENLIFETIME = 20; //minutes

        DatabaseConnection connection = new DatabaseConnection();

        public string Authenticate(string username, string password)
        {
            string token = "";
            if(username.Any(character => !Char.IsLetterOrDigit(character)) || password.Any(ch => !Char.IsLetterOrDigit(ch)))
            {
                return token;
            }

            IList<object[]> results = connection.RunQuery("SELECT Username FROM Users WHERE username = '" + username + "' AND password = '" + password + "'");

            if (results.Any())
            {
                token = Guid.NewGuid().ToString();
                string expirationDate = DateTime.UtcNow.AddMinutes(TOKENLIFETIME).ToString("yyyyMMdd HH: MM:ss");
                if (connection.RunNonQuery("UPDATE Users SET token='" + token + "', token_expirationDate='" + expirationDate + "' WHERE username = '" + username + "'"))
                {
                    return token;
                }
                else
                {
                    token = "";
                }
            }

            return token;
        }

        public string Register(string username)
        {
            string password = "";
            if (username.Any(character => !Char.IsLetterOrDigit(character)))
            {
                return password;
            }

            if (!AccountExists(username))
            {
                password = Guid.NewGuid().ToString().Replace("-", "").Substring(0, MINIMALPASSWORDLENGTH);

                if (connection.RunNonQuery("INSERT INTO Users(username, password) VALUES('" + username + "', '" + password + "')"))
                {
                    return password;
                }

                return password = "";
            }

            return password;
        }

        private bool AccountExists(string username)
        {
            IList<object[]> result = connection.RunQuery("SELECT * FROM Users WHERE username = '" + username + "'");

            return result.Count > 0;
        }
    }
}