using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tryout_Respond.Models
{
    public class UserManager
    {
        private DatabaseConnection databaseConnection;
        private Misc misc;

        public UserManager(DatabaseConnection databaseConnection, Misc misc)
        {
            this.databaseConnection = databaseConnection;
            this.misc = misc;
        }

        public bool ChangePassword(string token, string userID, string unencryptedNewPassword)
        {
            var success = false;

            if (unencryptedNewPassword.Length < Constants.MINIMALPASSWORDLENGTH || unencryptedNewPassword.Length > Constants.MAXIMUMPASSWORDLENGTH)
            {
                return success = false;
            }

            string passwordHash = misc.HashPassword(unencryptedNewPassword);

            success = databaseConnection.ChangePassword(userID, passwordHash);

            misc.DeleteToken(token);

            return success;
        }

        public User GetUser(string username)
        {
            return databaseConnection.GetUser(username);
        }
        
        public User GetUserByID(string userID)
        {
            return databaseConnection.GetUserByUserID(userID);
        }

        public User GetUserByIDWithToken(string userID)
        {
            return databaseConnection.GetUserByUserIDWithToken(userID);
        }

        public bool MakeAdmin(string userID, bool isAdmin)
        {
            return databaseConnection.MakeAdmin(userID, isAdmin);
        }

        public IList<User> GetAccounts()
        {
            return databaseConnection.GetUsers();
        }

        public bool SetUsername(string token, string username)
        {
            return databaseConnection.SetUsername(token, username);
        }
    }
}