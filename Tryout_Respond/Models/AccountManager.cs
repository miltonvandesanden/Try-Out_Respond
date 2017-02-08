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
        public const string AVAILABLEPASSWORDCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        DatabaseConnection connection = new DatabaseConnection();

        public string Authenticate(string username, string password)
        {
            if(username.Any(ch => !Char.IsLetterOrDigit(ch)) || password.Any(ch => !Char.IsLetterOrDigit(ch)))
            {
                return "";
            }

            List<object[]> result = connection.RunQuery("SELECT * FROM Users WHERE username = '" + username + "' AND password = '" + password + "'");

            string token = "";

            foreach (object[] value in result)
            {
                token = value[1].ToString() + "|" + DateTime.UtcNow.ToString();
            }

            return token;
        }

        public string Register(string username)
        {
            if (username.Any(ch => !Char.IsLetterOrDigit(ch)))
            {
                return "";
            }

            if (!AccountExists(username))
            {
                string password = GeneratePassword();

                if (connection.RunNonQuery("INSERT INTO Users(username, password) VALUES('" + username + "', '" + password + "')"))
                {
                    return password;
                }

                return "";
            }

            return "";
        }

        private bool AccountExists(string username)
        {
            List<object[]> result = connection.RunQuery("SELECT * FROM Users WHERE username = '" + username + "'");

            return result.Count > 0;
        }

        private string GeneratePassword()
        {
            char[] stringChars = new char[MINIMALPASSWORDLENGTH];

            for(int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = AVAILABLEPASSWORDCHARS[SecureRandom.Next(0, AVAILABLEPASSWORDCHARS.Length)];
            }

            return new String(stringChars);
        }
    }
}