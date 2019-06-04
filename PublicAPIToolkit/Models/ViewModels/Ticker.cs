using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models.ViewModels
{
   public class Ticker
   {
      public string Symbol { get; set; }
      public decimal Price { get; set; }
   }
}