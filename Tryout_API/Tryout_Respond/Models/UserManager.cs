using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tryout_Respond.Models
{
    public class UserManager
    {
        private DatabaseConnection databaseConnection = new DatabaseConnection();
        private Misc misc = new Misc();
        private LoginManager loginManager = new LoginManager();

        public bool ChangePassword(string token, string userID, string unencryptedNewPassword)
        {
            var success = false;

            if (unencryptedNewPassword.Length < Constants.MINIMALPASSWORDLENGTH || unencryptedNewPassword.Length > Constants.MAXIMUMPASSWORDLENGTH)
            {
                return success = false;
            }

            string passwordHash = misc.HashPassword(unencryptedNewPassword);

            success = databaseConnection.ChangePassword(userID, passwordHash);

            loginManager.DeleteToken(token);

            return success;
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

        public bool MakeAdmin(string userID, bool isAdmin)
        {
            return databaseConnection.MakeAdmin(userID, isAdmin);
        }

        public IList<object[]> GetUserIDs()
        {
            return databaseConnection.GetUserIDs();
        }
    }
}