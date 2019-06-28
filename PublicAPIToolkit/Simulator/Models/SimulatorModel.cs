using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Simulator.Models
{
   public class SimulatorModel
   {
      public class SimulatorMessage
      {
         public int InfoMessageGroupId { get; private set; }
         public ESimulatorMessageDescriptor InfoMessageDescriptor { get; set; }
         public string Message { get; set; }
         private static int nextInfoMessageGroupId = -1;

         public SimulatorMessage(bool nextGroup, ESimulatorMessageDescriptor infoMessageDescriptor, string message)
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
            nextInfoMessageGroupId++;
            InfoMessageGroupId = nextInfoMessageGroupId;
         }
      }
      public List<SimulatorMessage> infoMessageList;

      public SimulatorModel()
      {
         infoMessageList = new List<SimulatorMessage>();
      }
   }
}