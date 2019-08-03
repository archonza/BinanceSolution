using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Toolkit.Models
{
   public class ToolkitInputModel
   {
      public String EndPoint { get; set; } = String.Empty;
      public UInt32 HttpMethod { get; set; } = 0;
   }
}