﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Login.Models
{
   public class LoginInputModel
   {
      public string UserName { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
   }
}