using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models
{
   public class InfoModel
   {
      public class InfoMessage
      {
         public EInfoMessageId Id { get; set; }
         public string Message { get; set; }

         public InfoMessage(EInfoMessageId id, string message)
         {
            Id = id;
            Message = message;
         }
      }
      public List<InfoMessage> infoMessageList;

      public InfoModel()
      {
         infoMessageList = new List<InfoMessage>();
      }
   }
}