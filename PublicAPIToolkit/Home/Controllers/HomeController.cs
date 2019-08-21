using PublicAPIToolkit.Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit.Home.Controllers
{
   public class HomeController : Controller
   {
      public ActionResult Index()
      {
         ViewBag.Title = "Home Page";
         ViewData["LoginViewModel"] = new LoginViewModel()
         {
            LoggedIn = false
         };

         return View();
      }
   }
}
