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
        //[Route("")]
        public HttpResponseMessage Get/*Accounts*/()
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token) || !misc.IsAdmin(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                var userInfo = String.Empty;

                foreach (object[] userID in userManager.GetUserIDs())
                {
                    userInfo += userManager.GetAccountInfo(userID[0].ToString(), token);
                    userInfo += Environment.NewLine;
                }

                return Request.CreateResponse(HttpStatusCode.OK, userInfo);
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
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string ownAccountInfo = userManager.GetAccountInfo(userID, token);

                if (String.IsNullOrWhiteSpace(ownAccountInfo))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                return Request.CreateResponse(HttpStatusCode.OK, ownAccountInfo);
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
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string token = Request.Headers.GetValues("token").FirstOrDefault();

                if (String.IsNullOrWhiteSpace(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token) || !misc.IsAdmin(token) || !misc.AccountExistsUserID(userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                var isAdmin = true;

                if (!userManager.MakeAdmin(userID, isAdmin))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                return Request.CreateResponse(HttpStatusCode.OK, userID + "set to admin");
            }
            catch (InvalidOperationException invalidOperationException)
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
                if (!Request.Headers.GetValues("token").Any() || !Request.Headers.GetValues("newPassword").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string token = Request.Headers.GetValues("token").SingleOrDefault();
                string unencryptedNewPassword = Request.Headers.GetValues("newPassword").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(token) || String.IsNullOrWhiteSpace(unencryptedNewPassword))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(token) || !misc.IsAccountOwner(token, userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!userManager.ChangePassword(token, userID, unencryptedNewPassword))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "logout failed");
                }

                return Request.CreateResponse(HttpStatusCode.OK, unencryptedNewPassword);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }
    }
}
