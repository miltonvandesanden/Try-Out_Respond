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
            var token = String.Empty;

            if(username.Any(character => !Char.IsLetterOrDigit(character)) || password.Any(character => !Char.IsLetterOrDigit(character)))
            {
                return token;
            }

            IList<object[]> results = connection.RunQuery("SELECT Username FROM Users WHERE username = '" + username + "' AND password = '" + password + "'");

            if (!results.Any())
            {
                return token;
            }

            token = Guid.NewGuid().ToString();
            var expirationDate = DateTime.UtcNow.AddMinutes(TOKENLIFETIME).ToString("yyyyMMdd HH: MM:ss");

            if (!connection.RunNonQuery("UPDATE Users SET token='" + token + "', token_expirationDate='" + expirationDate + "' WHERE username = '" + username + "'"))
            {
                return token = String.Empty;
            }

            return token;

        }

        public string Register(string username)
        {
            var password = String.Empty;

            if (username.Any(character => !Char.IsLetterOrDigit(character)))
            {
                return password;
            }

            if (AccountExists(username))
            {
                return password;
            }

            password = Guid.NewGuid().ToString().Replace("-", "").Substring(0, MINIMALPASSWORDLENGTH);

            if (!connection.RunNonQuery("INSERT INTO Users(username, password) VALUES('" + username + "', '" + password + "')"))
            {
                return password = String.Empty;
            }

            return password;
        }

        private bool AccountExists(string username)
        {
            IList<object[]> result = connection.RunQuery("SELECT userID FROM Users WHERE username = '" + username + "'");

            return result.Any();
        }

        public bool isTokenValid(string token)
        {
            IList<object[]> results = connection.RunQuery("SELECT token FROM Users WHERE token = '" + token + "' AND token_expirationDate > '" + DateTime.UtcNow.ToString("yyyyMMdd HH: MM:ss") + "'");

            return results.Any();
        }

        public String GetOwnAccountInfo(string token)
        {
            var accountInfo = String.Empty;

            IList<object[]> results = connection.RunQuery("SELECT username, password FROM Users WHERE token = '" + token + "'");

            foreach (object accountField in results.First())
            {
                accountInfo += accountField + ":";
            }

            return accountInfo;
        }

        public String RefreshToken(string oldToken)
        {
            var newToken = Guid.NewGuid().ToString();
            var expirationDate = DateTime.UtcNow.AddMinutes(TOKENLIFETIME).ToString("yyyyMMdd HH: MM:ss");

            if (!connection.RunNonQuery("UPDATE Users SET token='" + newToken + "', token_expirationDate='" + expirationDate + "' WHERE token = '" + oldToken + "'"))
            {
                return newToken = String.Empty;
            }

            return newToken;
        }
    }
}