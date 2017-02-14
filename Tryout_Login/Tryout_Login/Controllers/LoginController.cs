using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tryout_Login.Models;
using static System.Collections.Specialized.BitVector32;

namespace Tryout_Login.Controllers
{
    public class LoginController : Controller
    {
        string prefixLogin = "http://localhost:54295/api/login/";
        string prefixUser = "http://localhost:54295/api/user/";
        string token;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ViewResult InfoAllAccounts()
        {
            PostRefreshToken();
            GetAllAccounts();

            return View();
        }

        public ViewResult Registreer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registreer(LoginViewModel model)
        {
            PostRegister(model);

            return View("~/Views/Login/Login_Result.cshtml");
        }

        public ViewResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            PostAuth(model);

            return View("~/Views/Login/Login_Result.cshtml");
        }

        public void PostRegister(LoginViewModel model)
        {

            string url = prefixLogin + "register";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("username", model.username);
            request.Method = "POST";
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Items = "Je nieuwe wachtwoord is: " + result;

        }

        public void PostAuth(LoginViewModel model)
        {
            string url = prefixLogin + "auth";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.UseDefaultCredentials = false;

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(model.username + ":" + model.password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            token = result;

            Session["currentToken"] = token;

            ViewBag.Items = result;
        }

        public void PostRefreshToken()
        {
            token = Session["currentToken"].ToString();
            token = token.TrimEnd('"');
            token = token.TrimStart('"');
            string url = prefixLogin + "refreshToken";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers.Add("token", token);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            token = result;
            Session["currentToken"] = token;
            token = Session["currentToken"].ToString();
            token = token.TrimEnd('"');
            token = token.TrimStart('"');
        }

        public void PostLogout()
        {
            string url = prefixLogin + "logout";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers.Add("token", token);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Logout = "Je bent uitgelogd" + result;
        }

        public void PostMakeAdmin()
        {
            string url = prefixUser + "{userID}/makeadmin";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers.Add("token", token);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Logout = "Deze gebruiker is nu admin " + result;
        }

        public void GetUserID()
        {
            string url = prefixUser + "{userID}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.Headers.Add("token", token);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Logout = "Account info: " + result;
        }

        public void PostChangePassword(LoginViewModel model)
        {
            string url = prefixUser + "changePassword";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers.Add("token", token);
            request.Headers.Add("password", model.password);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Logout = "Je wachtwoord is gewijzigd: " + result;
        }

        public void GetAllAccounts()
        {
            string url = prefixUser + "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.Headers.Add("token", token);
            request.ContentLength = 0;

            string result;

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            ViewBag.Logout = "Account info: " + result;
        }

    }
}