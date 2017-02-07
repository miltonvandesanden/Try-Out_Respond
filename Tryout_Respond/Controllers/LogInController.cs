using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tryout_Respond.Models;

namespace Tryout_Respond.Controllers
{
    public class LogInController : ApiController
    {
        public bool Get()
        {
            string username = "milton2";
            string password = "tilburg";

            return (new AccountManager()).LogIn(username, password);
        }
    }
}
