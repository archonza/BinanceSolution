using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Registration.Models
{
   public class RegistrationInputModel
   {
      public string FirstName { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public string Country { get; set; } = string.Empty;
      public string ContactNumber { get; set; } = string.Empty;
      public string IdNumber { get; set; } = string.Empty;
      public string UserName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public string RepeatPassword { get; set; } = string.Empty;
   }
}