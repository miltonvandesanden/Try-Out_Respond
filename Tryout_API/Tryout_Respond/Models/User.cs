using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tryout_Respond.Models
{
    public class User
    {
        public string userID { get; set; }
        public string username { get; set; }
        public string passwordHash { get; set; }
        public bool isAdmin { get; set; }
        public string token { get; set; }
        public DateTime tokenExpirationDate { get; set; }
    }
}