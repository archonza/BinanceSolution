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
         /* TODO: Check whether user is not already logged in */
         ViewData["LoginViewModel"] = new LoginViewModel()
         {
            LoggedIn = false
         };
         return View("~/Home/Views/Index.cshtml");
      }

      [HttpPost]
      public JsonResult Login(LoginInputModel loginInputModel)
      {
         if (Models.Login.Authorization(loginInputModel.UserName, loginInputModel.Password) == true)
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = true
            };
            /* Only add if not in object */
            logins.Add(new Models.Login(loginInputModel.UserName, loginInputModel.Password, true, GetClientIpAddress()));
            /* Only sync if not already logged in */
            DbSync();
         }
         else
         {
            ViewData["LoginViewModel"] = new LoginViewModel()
            {
               LoggedIn = false
            };
            //logins.RemoveAll(x => x.UserName == loginInputModel.UserName);
            //DbSync();
         }

         return Json(ViewData["LoginViewModel"], JsonRequestBehavior.AllowGet);
      }

      private string GetClientIpAddress()
      {
         string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
         if (string.IsNullOrEmpty(ipAddress))
         {
            ipAddress = Request.ServerVariables["REMOTE_ADDR"];
         }

         return ipAddress;
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

         if (logins[logins.Count - 1].LoggedIn == true)
         {
            // NOTE: Id primary key is automatically generated
            databaseController.InsertInto(
               "dbo.Login",
               databaseController.SelectFromTableWhereColumns("dbo.Users", "UserId", "UserName", logins[logins.Count - 1].UserName)[0], // Get UserId from Users database
               logins[logins.Count - 1].UserName,
               logins[logins.Count - 1].Password,
               (Convert.ToUInt32(logins[logins.Count - 1].LoggedIn)).ToString(),
               logins[logins.Count - 1].IpAddress);
            logins[logins.Count - 1].LoginId = Convert.ToInt32(databaseController.Select("SELECT LoginId FROM dbo.Login WHERE(UserName = '" + logins[logins.Count - 1].UserName + "' AND IpAddress = '" + logins[logins.Count - 1].IpAddress + "');")[0]);
         }
      }
   }
}