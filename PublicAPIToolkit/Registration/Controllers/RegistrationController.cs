using PublicAPIToolkit.Database.Controllers;
using PublicAPIToolkit.Registration.Models;
using PublicAPIToolkit.User.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit.Registration.Controllers
{
   public class RegistrationController : Controller
   {
      private Models.Registration registration;
      private RegistrationViewModel registrationViewModel;
      private DatabaseController databaseController;

      public RegistrationController()
      {
         databaseController = DatabaseController.GetInstance();
         registration = new Models.Registration();
         registrationViewModel = new RegistrationViewModel();
      }

      // GET: Registration
      public ActionResult Index()
      {
         ViewBag.Title = "Registration";
         return View();
      }

      public ActionResult Success()
      {
         ViewBag.Title = "Success";
         return View();
      }

      public ActionResult Confirmation()
      {
         ViewBag.Title = "Confirmation";
         return View();
      }

      [HttpPost]
      public JsonResult Register(RegistrationInputModel registrationInputModel)
      {
         registration.UserName = registrationInputModel.UserName;
         registration.Password = registrationInputModel.Password;
         registration.RepeatPassword = registrationInputModel.RepeatPassword;
         registrationViewModel.status = registration.Verify();
         if (registrationViewModel.status == ERegistrationStatus.Successfull)
         {
            UserController userController = new UserController();
            PublicAPIToolkit.User.Models.User user = new PublicAPIToolkit.User.Models.User();
            user.FirstName = registrationInputModel.FirstName;
            user.LastName = registrationInputModel.LastName;
            user.Country = registrationInputModel.Country;
            user.ContactNumber = registrationInputModel.ContactNumber;
            user.NID = registrationInputModel.IdNumber;
            user.UserName = registrationInputModel.UserName;
            user.Password = registrationInputModel.Password;
            userController.AddUser(user);
         }
         return Json(registrationViewModel, JsonRequestBehavior.AllowGet);
      }
   }
}