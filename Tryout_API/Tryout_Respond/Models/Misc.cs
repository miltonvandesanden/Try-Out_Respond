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
        private DatabaseConnection databaseConnection;

        public Misc(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public string HashPassword(string unencryptedPassword)
        {
            return Encoding.ASCII.GetString(new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(unencryptedPassword)));
        }

        public bool IsTokenValid(string token)
        {
            return databaseConnection.IsTokenValid(token);
        }

        public bool DeleteToken(string token)
        {
            return databaseConnection.DeleteToken(token);
        }
    }
}