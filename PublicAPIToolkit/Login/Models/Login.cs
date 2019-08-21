using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Login.Models
{
   public class Login
   {
      public string UserName { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public bool LoggedIn { get; set; } = false;

      public bool Authorization(string userName, string password)
      {
         // Search db and verify whether username and password is correct.
         return true;
      }

      public void DbSync()
      {
         // Once logged in, sync with db.
         if (LoggedIn == true)
         {

         }
      }
   }
}