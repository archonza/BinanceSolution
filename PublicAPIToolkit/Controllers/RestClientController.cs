using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PublicAPIToolkit.Models.DomainModels.Rest;
using System.Net;
using System.IO;

namespace PublicAPIToolkit.Controllers
{
   public class RestClientController
   {
      private RestClient restClient;

      public RestClientController(string endPoint, uint httpMethod)
      {
         this.restClient = new RestClient();
         this.restClient.EndPoint = endPoint;
         this.restClient.HttpMethod = (EHttpMethod)httpMethod;
         this.restClient.ResponseData = string.Empty;
      }

      public void MakeRequest()
      {
         string responseValue = string.Empty;

         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(restClient.EndPoint);

         request.Method = restClient.HttpMethod.ToString();

         HttpWebResponse response = null;

         try
         {
            response = (HttpWebResponse)request.GetResponse();


            //Proecess the resppnse stream... (could be JSON, XML or HTML etc..._

            using (Stream responseStream = response.GetResponseStream())
            {
               if (responseStream != null)
               {
                  using (StreamReader reader = new StreamReader(responseStream))
                  {
                     responseValue = reader.ReadToEnd();
                  }
               }
            }
         }
         catch (Exception ex)
         {
            responseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
         }
         finally
         {
            if (response != null)
            {
               ((IDisposable)response).Dispose();
            }
         }

         restClient.ResponseData =  responseValue;
      }

      public string GetResponse()
      {
         return restClient.ResponseData;
      }
   }
}