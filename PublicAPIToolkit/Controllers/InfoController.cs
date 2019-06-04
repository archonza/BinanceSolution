using PublicAPIToolkit.Models;
using System;

namespace PublicAPIToolkit.Controllers
{
    public class InfoController
    {
      private InfoModel infoModel;
      private string FullFilePath { get; set; }
      
      public InfoController(InfoModel infoModel, string fullFilePath)
      {
         FullFilePath = fullFilePath;
         this.infoModel = infoModel;
      }

      public void Print(EInfoMessageId infoMessageId)
      {
         using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(@FullFilePath, true))
         {
            if (FullFilePath != null)
            {
               file.WriteLine(infoModel.infoMessageList.Find(x => x.Id == infoMessageId).Message);
            }
         }
      }
   }
}