using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.User.Models
{
   public class User
   {
      public uint Id { get; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Country { get; set; }
      public string ContactNumber { get; set; }
      public string NID { get; set; }
      public string UserName { get; set; }
      public string Email { get; set; }
      public string Password { get; set; }
   }
}