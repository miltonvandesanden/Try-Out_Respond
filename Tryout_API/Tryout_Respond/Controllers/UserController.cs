using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tryout_Respond.Models;

namespace Tryout_Respond.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private LoginManager loginManager = new LoginManager();
        private UserManager userManager = new UserManager();
        private Misc misc = new Misc();

        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get/*Accounts*/()
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token)/* || !misc.IsAdmin(token)*/)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                return Request.CreateResponse(HttpStatusCode.OK, userManager.GetAccounts().ToArray());
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpGet]
        [Route("{userID}")]
        public HttpResponseMessage GetAccount(string userID)
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                IList<string> ownAccountInfo = userManager.GetAccountInfo(userID, token);

                /*if (String.IsNullOrWhiteSpace(ownAccountInfo))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }*/

                return Request.CreateResponse(HttpStatusCode.OK, ownAccountInfo.ToArray());
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("{userID}/makeadmin")]
        public HttpResponseMessage MakeAdmin(string userID)
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                string token = Request.Headers.GetValues("token").FirstOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                if(!misc.IsAdmin(token) || !misc.AccountExistsUserID(userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "no access rights");
                }

                var isAdmin = true;

                if (!userManager.MakeAdmin(userID, isAdmin))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(userID + " set to admin");

                return response;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("{userID}/changeUsername")]
        public HttpResponseMessage ChangeUsername(string userID)
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                if (!Request.Headers.GetValues("newUsername").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();
                string newUsername = Request.Headers.GetValues("newUsername").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token) || String.IsNullOrWhiteSpace(newUsername))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                if (!misc.IsAccountOwner(token, userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "access forbidden");
                }

                if(misc.AccountExistsUsername(newUsername))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "username in use");
                }

                if (!userManager.SetUsername(token, newUsername))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "username change failed");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(newUsername);

                return response;
            }
            catch (InvalidOperationException invalidOperationsException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("{userID}/changePassword")]
        public HttpResponseMessage ChangePassword(string userID)
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                if (!Request.Headers.GetValues("newPassword").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();
                string unencryptedNewPassword = Request.Headers.GetValues("newPassword").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token) || String.IsNullOrWhiteSpace(unencryptedNewPassword))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                if(!misc.IsAccountOwner(token, userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "no access rights");
                }

                if (!userManager.ChangePassword(token, userID, unencryptedNewPassword))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "failed");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(unencryptedNewPassword);

                return response;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }
    }
}
