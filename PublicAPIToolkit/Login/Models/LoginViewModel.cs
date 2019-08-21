using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Login.Models
{
   public class LoginViewModel
   {
      public bool LoggedIn { get; set; } = false;
      public string UserName { get; set; } = string.Empty;
   }
}