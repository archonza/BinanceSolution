﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Registration.Models
{
   public class Registration
   {
      public string UserName { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public string RepeatPassword { get; set; } = string.Empty;

      public ERegistrationStatus Verify()
      {
         return ERegistrationStatus.Successfull;
      }
   }
}