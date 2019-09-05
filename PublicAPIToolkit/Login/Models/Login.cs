using PublicAPIToolkit.Database.Controllers;
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

      public Login(string userName, string password, bool loggedIn)
      {
         UserName = userName;
         Password = password;
         LoggedIn = loggedIn;
      }

      public static bool Authorization(string userName, string password)
      {
         DatabaseController databaseController;
         bool result;
         databaseController = DatabaseController.GetInstance();
         if ((databaseController.SelectFromTableWhereColumns("dbo.Users", "UserName", "UserName", userName)[0] == userName) &&
             (databaseController.SelectFromTableWhereColumns("dbo.Users", "Password", "Password", password)[0] == password))
         {
            result = true;
         }
         else
         {
            result = false;
         }
         return result;
      }
   }
}