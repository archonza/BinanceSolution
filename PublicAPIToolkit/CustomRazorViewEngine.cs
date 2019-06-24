using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicAPIToolkit
{
   public class CustomRazorViewEngine : RazorViewEngine
   {
      public CustomRazorViewEngine()
      {
         ViewLocationFormats = new string[]
         {
            "~/{1}/Views/{0}.cshtml"
         };

         PartialViewLocationFormats = new string[]
         {
            "~/Shared/Views/{0}.cshtml"
         };
      }
   }
}