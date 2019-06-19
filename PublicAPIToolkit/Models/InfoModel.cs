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
         public int InfoMessageGroupId { get; private set; }
         public EInfoMessageDescriptor InfoMessageDescriptor { get; set; }
         public string Message { get; set; }
         private static int nextInfoMessageGroupId = -1;

         public InfoMessage(bool nextGroup, EInfoMessageDescriptor infoMessageDescriptor, string message)
         {
            InfoMessageDescriptor = infoMessageDescriptor;
            Message = message;
            if (nextGroup == true)
            {
               NextGroup();
            }
         }

         private void NextGroup()
         {
            InfoMessageGroupId = ++nextInfoMessageGroupId;
         }
      }
      public List<InfoMessage> infoMessageList;

      public InfoModel()
      {
         infoMessageList = new List<InfoMessage>();
      }
   }
}