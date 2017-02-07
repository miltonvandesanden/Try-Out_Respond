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
        public HttpResponseMessage Get()
        {
            string username = "milton2";
            string password = "tilburg";

            string token = (new AccountManager()).Authenticate(username, password);

            if(token == "")
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }
    }
}
