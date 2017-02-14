using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tryout_Login.Models;

namespace Tryout_Login.Controllers
{
    public class LoginController : Controller
    {
        string prefix = "http://localhost:54295/api/accounts/";

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
        public ActionResult Login(LoginViewModel model)
        {
            if (model.password == null)
            {
                PostRegister(model);
            }
            else
            {
                PostAuth(model);
            }

            return View("~/Views/Login/Login_Result.cshtml");

        }

        public void PostRegister(LoginViewModel model)
        {

            string url = prefix + "register";
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
            string url = prefix + "auth";
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

            ViewBag.Items = result;
        }
    }
}