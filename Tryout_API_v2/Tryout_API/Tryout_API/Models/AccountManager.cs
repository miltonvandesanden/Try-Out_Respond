using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Tryout_Respond.Models
{
    public class AccountManager
    {
        public const int MINIMALPASSWORDLENGTH = 8;
        public const int MAXIMUMPASSWORDLENGTH = 15;

        public const int TOKENLIFETIME = 20; //minutes

        DatabaseConnection databaseConnection = new DatabaseConnection();

        public string HashPassword(string unencryptedPassword)
        {
            return Encoding.ASCII.GetString(new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(unencryptedPassword)));
        }

        public string Authenticate(string username, string unencryptedPassword)
        {
            var token = String.Empty;

            if (username.Any(character => !Char.IsLetterOrDigit(character)) || unencryptedPassword.Any(character => !Char.IsLetterOrDigit(character)))
            {
                return token;
            }

            if (unencryptedPassword.Length < MINIMALPASSWORDLENGTH || unencryptedPassword.Length > MAXIMUMPASSWORDLENGTH)
            {
                return token;
            }

            var passwordHash = HashPassword(unencryptedPassword);

            if (!databaseConnection.IsAccountOwner(username, passwordHash))
            {
                return token;
            }

            token = Guid.NewGuid().ToString();
            var expirationDate = DateTime.UtcNow.AddMinutes(TOKENLIFETIME)/*.ToString("yyyyMMdd HH:mm")*/;

            if (!databaseConnection.SetToken(token, expirationDate, username))
            {
                return token = String.Empty;
            }

            return token;

        }

        public string Register(string username)
        {
            var passwordHash = String.Empty;

            if (username.Any(character => !Char.IsLetterOrDigit(character)))
            {
                return passwordHash;
            }

            if (databaseConnection.AccountExistsUsername(username))
            {
                return passwordHash;
            }

            var unencryptedPassword = Guid.NewGuid().ToString().Replace("-", "").Substring(0, MAXIMUMPASSWORDLENGTH);
            passwordHash = HashPassword(unencryptedPassword);

            string userID = GenerateUserID();

            if (!databaseConnection.InsertUser(userID, username, passwordHash))
            {
                return passwordHash = String.Empty;
            }

            return unencryptedPassword;
        }

        public string GenerateUserID()
        {
            var userID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 3);

            if (databaseConnection.AccountExistsUserID(userID))
            {
                userID = GenerateUserID();
            }

            return userID;
        }

        public bool isTokenValid(string token)
        {
            return databaseConnection.IsTokenValid(token);
        }

        public String GetAccountInfo(string userID, string token)
        {
            string accountInfo = databaseConnection.GetUsername(userID);

            if (databaseConnection.IsAccountOwnerByToken(userID, token))
            {
                accountInfo += databaseConnection.GetPasswordByUserID(userID);
            }

            return accountInfo;
        }

        public String RefreshToken(string oldToken)
        {
            var newToken = Guid.NewGuid().ToString();
            var expirationDate = DateTime.UtcNow.AddMinutes(TOKENLIFETIME)/*.ToString("yyyyMMdd HH:mm")*/;

            if (!databaseConnection.RefreshToken(oldToken, newToken, expirationDate))
            {
                return newToken = String.Empty;
            }

            return newToken;
        }

        public bool DeleteToken(string token)
        {
            return databaseConnection.DeleteToken(token);
        }

        public bool ChangePassword(string token, string unencryptedNewPassword)
        {
            var success = false;

            if (unencryptedNewPassword.Length < MINIMALPASSWORDLENGTH || unencryptedNewPassword.Length > MAXIMUMPASSWORDLENGTH)
            {
                return success = false;
            }

            string passwordHash = HashPassword(unencryptedNewPassword);

            success = databaseConnection.ChangePassword(token, passwordHash);

            DeleteToken(token);

            return success;
        }
    }
}