using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using Tryout_Respond.Models;

namespace Tryout_Respond.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private const string authorizationType = "Basic";
        private LoginManager loginManager = new LoginManager();
        private UserManager userManager = new UserManager();
        private Misc misc = new Misc();
        
    [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register()
        { 
            try
            {
                if (!Request.Headers.GetValues("username").Any())
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                string username = Request.Headers.GetValues("username").SingleOrDefault();

                bool isAdmin = false;
                string password = loginManager.Register(username);

                if (String.IsNullOrWhiteSpace(password))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                string JsonPassword = JsonConvert.SerializeObject(new { password = password });
    
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonPassword);

                return response;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
            }
        }

        [HttpPost]
        [Route("auth")]
        public HttpResponseMessage Auth()
        {
            if (Request.Headers.Authorization == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
            }

            if (!Request.Headers.Authorization.Scheme.Equals(authorizationType))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
            }

            string encodedUsernamePassword = Request.Headers.Authorization.Parameter;

            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var seperatorIndex = usernamePassword.IndexOf(":");

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            string token = loginManager.Authenticate(username, password);

            if (String.IsNullOrWhiteSpace(token))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
            }

            string JsonPassword = JsonConvert.SerializeObject(new { password = password });
            string JsonUsername = JsonConvert.SerializeObject(new { username = username});
            string JsonToken = JsonConvert.SerializeObject(new { token = token });
            string JsonIsAdmin = JsonConvert.SerializeObject(new { isAdmin = misc.IsAdmin(token) });

            string content = JsonPassword + JsonUsername + JsonToken + JsonIsAdmin;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(content);

            return response;
        }

        [HttpPost]
        [Route("logout")]
        public HttpResponseMessage Logout()
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
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "invalid credentials");
                }

                if (!misc.IsTokenValid(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                if (!loginManager.DeleteToken(token))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "logout failed");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(" logged out");

                return response;
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
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");
                }

                string oldToken = Request.Headers.GetValues("token").SingleOrDefault();

                if (String.IsNullOrWhiteSpace(oldToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                if (!misc.IsTokenValid(oldToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "session expired");
                }

                String newToken = loginManager.RefreshToken(oldToken);

                if (String.IsNullOrWhiteSpace(newToken))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
                }

                string content = JsonConvert.SerializeObject(new { newToken = newToken });

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(content);

                return response;
            }
            catch (InvalidOperationException invalidOperatonException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "credentials invalid");
            }
        }
    }
}