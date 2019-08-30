using PublicAPIToolkit.Database.Controllers;
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
      private static List<PublicAPIToolkit.Login.Models.Login> logins;
      private DatabaseController databaseController;

      public LoginController()
      {
         databaseController = DatabaseController.GetInstance();
         logins = new List<PublicAPIToolkit.Login.Models.Login>();
      }

      public ActionResult Index()
      {
         return View("~/Home/Views/Index.cshtml");
      }

      // GET
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
         if (Models.Login.Authorization(loginInputModel.UserName, loginInputModel.Password) == true)
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = true
            };
            logins.Add(new Models.Login(loginInputModel.UserName, loginInputModel.Password, true));
            DbSync();
         }
         else
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = false
            };
            logins.RemoveAll(x => x.UserName == loginInputModel.UserName);
            DbSync();
         }

         return View("~/Home/Views/Index.cshtml");
      }

      public ActionResult Logout(LoginInputModel loginInputModel)
      {
         ViewData["LoginViewModel"] = new LoginViewModel()
         {
            LoggedIn = false
         };
         logins.RemoveAll(x => x.UserName == loginInputModel.UserName);
         DbSync();
         return View("~/Home/Views/Index.cshtml");
      }

      public void DbSync()
      {
         // NOTE: Id primary key is automatically generated
         databaseController.InsertInto(
            "dbo.Login",
            "" // Get UserId from Users database
            logins[logins.Count - 1].UserName,
            logins[logins.Count - 1].Password,
            (Convert.ToUInt32(logins[logins.Count - 1].LoggedIn)).ToString());
      }
   }
}