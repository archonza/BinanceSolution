using PublicAPIToolkit.Simulator.Models;
using System;
using System.IO;
using System.Security.AccessControl;
using static PublicAPIToolkit.Simulator.Models.SimulatorModel;

namespace PublicAPIToolkit.Simulator.Controllers
{
    public class SimulatorController
    {
      private SimulatorModel simulatorModel;
      private string FullFilePath { get; set; }
      private Object threadLock = new Object();

      public SimulatorController(string fullFilePath)
      {
         FullFilePath = fullFilePath;
         simulatorModel = new SimulatorModel();
      }

      public void AddSimulator(bool nextGroup, ESimulatorMessageDescriptor simulatorMessageDescriptor, string message)
      {
         simulatorModel.simulatorMessageList.Add(new SimulatorMessage(nextGroup, simulatorMessageDescriptor, message));
      }

      public void Print(int simulatorMessageGroupId, ESimulatorMessageDescriptor simulatorMessageDescriptor)
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
                  file.WriteLine(simulatorModel.simulatorMessageList.Find(x => (x.SimulatorMessageGroupId == simulatorMessageGroupId) && (x.SimulatorMessageDescriptor == simulatorMessageDescriptor)).Message);
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

      public void PrintAllGroups(ESimulatorMessageDescriptor simulatorMessageDescriptor)
      {
         lock (threadLock)
         {
            FileStream fileStream = null;
            try
            {
               //FileSecurity fileSecurity = new FileSecurity();
               //fileSecurity.AddAccessRule(new FileSystemAccessRule(@"DESKTOP-9H8NQNT\rudol", FileSystemRights.Write, AccessControlType.Allow));
               //fileStream = new FileStream(@FullFilePath, FileMode.Append, FileSystemRights.Write, FileShare.Write, 4096, FileOptions.None, fileSecurity);
               fileStream = new FileStream(@FullFilePath, FileMode.Append);

               using (System.IO.StreamWriter file =
                  new System.IO.StreamWriter(fileStream))
               {
                  if (FullFilePath != null)
                  {
                     for (int simulatorMessageGroupId = 0; simulatorMessageGroupId < GetNumberOfGroups(); simulatorMessageGroupId++)
                     {
                        file.WriteLine(simulatorModel.simulatorMessageList.Find(x => (x.SimulatorMessageGroupId == simulatorMessageGroupId) && (x.SimulatorMessageDescriptor == simulatorMessageDescriptor)).Message);
                     }
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

      public int GetNumberOfGroups()
      {
         return (simulatorModel.simulatorMessageList[simulatorModel.simulatorMessageList.Count - 1].SimulatorMessageGroupId) + 1;
      }
   }
}