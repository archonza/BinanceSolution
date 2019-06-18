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
         public uint InfoMessageGroupId { get; private set; }
         public EInfoMessageDescriptor InfoMessageDescriptor { get; set; }
         public string Message { get; set; }
         private static uint nextInfoMessageGroupId = 0;

         public InfoMessage(EInfoMessageDescriptor infoMessageDescriptor, string message)
         {
            InfoMessageDescriptor = infoMessageDescriptor;
            Message = message;
         }

         public void NextGroup()
         {
            InfoMessageGroupId = nextInfoMessageGroupId++;
         }
      }
      public List<InfoMessage> infoMessageList;

      public InfoModel()
      {
         infoMessageList = new List<InfoMessage>();
      }
   }
}