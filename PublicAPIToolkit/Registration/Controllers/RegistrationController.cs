using PublicAPIToolkit.Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit.Registration.Controllers
{
   public class RegistrationController : Controller
   {
      // GET: Registration
      public ActionResult Index()
      {
         return View();
      }

      [HttpPost]
      public string Register(RegistrationInputModel tradeToolInputModel)
      {
         return "Somestring";
      }
   }
}