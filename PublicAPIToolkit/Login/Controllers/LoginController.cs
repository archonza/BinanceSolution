using PublicAPIToolkit.Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit.Login.Controllers
{
   public class LoginController : Controller
   {
      private Models.Login login;

      public LoginController()
      {
         login = new Models.Login();
      }

      public ActionResult Index()
      {
         return View("~/Home/Views/Index.cshtml");
      }

      [HttpGet]
      public ActionResult Login()
      {
         ViewData["LoginViewModel"] = new LoginViewModel()
         {
            LoggedIn = false
         };
         return View("~/Home/Views/Index.cshtml");
      }

      [HttpPost]
      public ActionResult Login(LoginInputModel loginInputModel)
      {
         if (login.Authorization(loginInputModel.UserName, loginInputModel.Password) == true)
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = true
            };
            login.DbSync();
         }
         else
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = false
            };
         }

         return View("~/Home/Views/Index.cshtml");
      }

      public ActionResult Logout()
      {
         ViewData["LoginViewModel"] = new LoginViewModel()
         {
            LoggedIn = false
         };
         login.DbSync();
         return View("~/Home/Views/Index.cshtml");
      }
   }
}