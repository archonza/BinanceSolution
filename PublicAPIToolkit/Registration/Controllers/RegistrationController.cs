﻿using PublicAPIToolkit.Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit.Registration.Controllers
{
   public class RegistrationController : Controller
   {
      private Models.Registration registration;
      private RegistrationViewModel registrationViewModel;
      public RegistrationController()
      {
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
         registration.Password = registrationInputModel.RepeatPassword;
         registrationViewModel.status = registration.Verify();
         if (registrationViewModel.status == ERegistrationStatus.Successfull)
         {
             /* Add new user to user db */
         }
         return Json(registrationViewModel, JsonRequestBehavior.AllowGet);
      }
   }
}