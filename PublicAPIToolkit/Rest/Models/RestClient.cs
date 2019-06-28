using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Rest.Models
{
   public class RestClient
   {
      public string EndPoint { get; set; } = string.Empty;
      public EHttpMethod HttpMethod { get; set; } = EHttpMethod.GET;
      public string ResponseData { get; set; } = string.Empty;

      public RestClient()
      {
         EndPoint = string.Empty;
         HttpMethod = EHttpMethod.GET;
         ResponseData = string.Empty;
      }
   }
}