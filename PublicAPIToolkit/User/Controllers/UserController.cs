using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PublicAPIToolkit.User.Models;
using PublicAPIToolkit.Database.Controllers;

namespace PublicAPIToolkit.User.Controllers
{
   public class UserController
   {
      private static List<PublicAPIToolkit.User.Models.User> users;
      DatabaseController databaseController;

      public UserController()
      {
         databaseController = DatabaseController.GetInstance();
         users = new List<PublicAPIToolkit.User.Models.User>();
      }

      public void AddUser(PublicAPIToolkit.User.Models.User user)
      {
         users.Add(user);
         DbSync();
      }

      public void Remove(uint id)
      {
         users.RemoveAll(x => x.Id == id);
      }

      public void DbSync()
      {
         // NOTE: Id primary key is automatically generated
         databaseController.InsertInto(
            "dbo.Users",
            users[users.Count - 1].FirstName,
            users[users.Count - 1].LastName,
            users[users.Count - 1].Country,
            users[users.Count - 1].ContactNumber,
            users[users.Count - 1].NID,
            users[users.Count - 1].UserName,
            users[users.Count - 1].Email,
            users[users.Count - 1].Password);
      }
   }
}