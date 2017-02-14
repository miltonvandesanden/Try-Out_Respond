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
    [RoutePrefix("api/accounts")]
    public class AccountController : ApiController
    {
        private const string authorizationType = "Basic";
        private AccountManager accountManager = new AccountManager();

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register()
        {
            try
            {
                if (!Request.Headers.GetValues("username").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string username = Request.Headers.GetValues("username").SingleOrDefault();

                bool isAdmin = false;
                string password = accountManager.Register(username);

                if (String.IsNullOrWhiteSpace(password))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                return Request.CreateResponse(HttpStatusCode.OK, password);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("auth")]
        public HttpResponseMessage Auth()
        {
            if (Request.Headers.Authorization == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Authorization invalid");
            }

            if (!Request.Headers.Authorization.Scheme.Equals(authorizationType))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Authorization invalid");
            }

            string encodedUsernamePassword = Request.Headers.Authorization.Parameter;

            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var seperatorIndex = usernamePassword.IndexOf(":");

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            string token = accountManager.Authenticate(username, password);

            if (String.IsNullOrWhiteSpace(token))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [HttpPost]
        [Route("logout")]
        public HttpResponseMessage Logout()
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

                if (!accountManager.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!accountManager.DeleteToken(token))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "logout failed");
                }

                return Request.CreateResponse(HttpStatusCode.OK, "logged out");
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("refreshToken")]
        public HttpResponseMessage RefreshToken()
        {
            try
            {
                if (!Request.Headers.GetValues("token").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string oldToken = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(oldToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!accountManager.IsTokenValid(oldToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                String newToken = accountManager.RefreshToken(oldToken);

                if (String.IsNullOrWhiteSpace(newToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                return Request.CreateResponse(HttpStatusCode.OK, newToken);
            }
            catch (InvalidOperationException invalidOperatonException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage GetAccounts()
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

                if (!accountManager.IsTokenValid(token) || !accountManager.IsAdmin(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                var userInfo = String.Empty;

                foreach (object[] userID in accountManager.GetUserIDs())
                {
                    userInfo += accountManager.GetAccountInfo(userID[0].ToString(), token);
                    userInfo += Environment.NewLine;
                }

                return Request.CreateResponse(HttpStatusCode.OK, userInfo);
            }
            catch(InvalidOperationException invalidOperationException)
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

                if (!accountManager.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string ownAccountInfo = accountManager.GetAccountInfo(userID, token);

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

                if (!accountManager.IsTokenValid(token) || !accountManager.IsAdmin(token) || !accountManager.AccountExistsUserID(userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                var isAdmin = true;

                if (!accountManager.MakeAdmin(userID, isAdmin))
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

                if (!accountManager.IsTokenValid(token) || !accountManager.IsAccountOwner(token, userID))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!accountManager.ChangePassword(token, userID, unencryptedNewPassword))
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