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
        [Route("register")]
        public HttpResponseMessage Register()
        {
            if (!Request.Headers.GetValues("username").Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "credentials invalid");
            }

            string username = Request.Headers.GetValues("username").SingleOrDefault();

            string password = accountManager.Register(username);

            if (String.IsNullOrWhiteSpace(password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "invalid credentials");
            }

            return Request.CreateResponse(HttpStatusCode.OK, password);
        }

        [HttpPost]
        [Route("refreshToken")]
        public HttpResponseMessage RefreshToken()
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

            if (!accountManager.isTokenValid(oldToken))
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

        [HttpPost]
        [Route("{userID}")]
        public HttpResponseMessage GetAccountInfo(string userID)
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

            if (!accountManager.isTokenValid(token))
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

        [HttpPost]
        [Route("logout")]
        public HttpResponseMessage Logout()
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

            if (!accountManager.isTokenValid(token))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }

            if(!accountManager.DeleteToken(token))
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "logout failed");
            }

            return Request.CreateResponse(HttpStatusCode.OK, "logged out");
        }
    }
}
