using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Tryout_Respond.Models
{
    public class Misc
    {
        private DatabaseConnection databaseConnection = new DatabaseConnection();

        public string HashPassword(string unencryptedPassword)
        {
            return Encoding.ASCII.GetString(new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(unencryptedPassword)));
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

        public bool IsTokenValid(string token)
        {
            return databaseConnection.IsTokenValid(token);
        }

        public bool AccountExistsUsername(string username)
        {
            return databaseConnection.AccountExistsUsername(username);
        }

        public bool AccountExistsUserID(string userID)
        {
            return databaseConnection.AccountExistsUserID(userID);
        }

        public bool IsAdmin(string token)
        {
            return databaseConnection.IsAdmin(token);
        }

        public bool IsAccountOwner(string token, string userID)
        {
            return databaseConnection.IsAccountOwnerByToken(userID, token);
        }

        public string GetUserID(string token)
        {
            return databaseConnection.GetUserIDWithToken(token);
        }

        public string GetUsername(string userID)
        {
            return databaseConnection.GetUsername(userID);
        }
    }
}