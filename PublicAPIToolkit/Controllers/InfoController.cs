using PublicAPIToolkit.Models;
using System;
using System.IO;
using System.Security.AccessControl;
using static PublicAPIToolkit.Models.InfoModel;

namespace PublicAPIToolkit.Controllers
{
    public class InfoController
    {
      private InfoModel infoModel;
      private string FullFilePath { get; set; }
      
      public InfoController(string fullFilePath)
      {
         FullFilePath = fullFilePath;
         infoModel = new InfoModel();
      }

      public void AddInfo(EInfoMessageDescriptor infoMessageDescriptor, string message)
      {
         infoModel.infoMessageList.Add(new InfoMessage(infoMessageDescriptor, message));
      }

      public void Print(EInfoMessageDescriptor infoMessageDescriptor)
      {
         FileStream fileStream = null;
         try
         {
            FileSecurity fileSecurity = new FileSecurity();
            fileSecurity.AddAccessRule(new FileSystemAccessRule(@"DESKTOP-9H8NQNT\rudol", FileSystemRights.Write, AccessControlType.Allow));
            fileStream = new FileStream(@FullFilePath, FileMode.Append, FileSystemRights.Write, FileShare.Write, 4096, FileOptions.None, fileSecurity);

            using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(fileStream))
            {
               if (FullFilePath != null)
               {
                  file.WriteLine(infoModel.infoMessageList.Find(x => x.InfoMessageDescriptor == infoMessageDescriptor).Message);
               }
            }
         }
         finally
         {
            if (fileStream != null)
            {
               fileStream.Dispose();
            }

         }
      }
   }
}