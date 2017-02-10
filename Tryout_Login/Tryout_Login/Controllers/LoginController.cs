using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tryout_Login.Models;

namespace Tryout_Login.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public void Login(LoginViewModel model)
        {
            if (model.password == null)
            {
                PostRegister(model);
            }
            else
            {
                PostAuth(model);
            }
        }

        [HttpPost]
        //[Route("api/accounts/register")]
        public HttpWebRequest PostRegister(LoginViewModel model)
        {
            string url = "http://localhost:54295/api/accounts/register";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("username", model.username);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return request;
        }

        public HttpWebRequest PostAuth(LoginViewModel model)
        {
            string url = "http://localhost:54295/api/accounts/register";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Credentials = GetCredentials(model);
            request.PreAuthenticate = true;
            request.UseDefaultCredentials = false;
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(model.username + ":" + model.password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return request;
        }

        public CredentialCache GetCredentials(LoginViewModel model)
        {
            string username = model.username;
            string password = model.password;

            string url = "http://localhost:54295/api/accounts/auth";
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(url), "Basic", new NetworkCredential(username, password));
            return credentialCache;
        }
    }
}