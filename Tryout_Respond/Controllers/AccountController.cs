using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Tryout_Respond.Models;

namespace Tryout_Respond.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private const string authorizationType = "Basic";

        [HttpPost]
        [Route("auth")]
        public HttpResponseMessage Auth()
        {
            if(Request.Headers.Authorization != null)
            {
                if(Request.Headers.Authorization.Scheme == authorizationType)
                {
                    string encodedUsernamePassword = Request.Headers.Authorization.Parameter; 

                    //Encoding encoding = Encoding.GetEncoding("iso-859-1");
                    string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(":");

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    string password = usernamePassword.Substring(seperatorIndex + 1);

                    string token = (new AccountManager()).Authenticate(username, password);

                    if(token == "")
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, token);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Authorization invalid");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Authorization invalid");
            }

            /*string token = (new AccountManager()).Authenticate(username, password);

            if(token == "")
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return Request.CreateResponse(HttpStatusCode.OK, token);*/
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register()
        {
            if(Request.Headers.GetValues("username").Count() > 0)
            {
                string username = Request.Headers.GetValues("username").First();

                string password = (new AccountManager()).Register(username);

                if (password == "")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "invalid credentials");
                }

                return Request.CreateResponse(HttpStatusCode.OK, password);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "credentials invalid");
        }
    }
}
