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
         public int SimulatorMessageGroupId { get; private set; }
         public ESimulatorMessageDescriptor SimulatorMessageDescriptor { get; set; }
         public string Message { get; set; }
         private static int nextSimulatorMessageGroupId = -1;

         public SimulatorMessage(bool nextGroup, ESimulatorMessageDescriptor simulatorMessageDescriptor, string message)
         {
            SimulatorMessageDescriptor = simulatorMessageDescriptor;
            Message = message;
            if (nextGroup == true)
            {
               NextGroup();
            }
         }

         private void NextGroup()
         {
            nextSimulatorMessageGroupId++;
            SimulatorMessageGroupId = nextSimulatorMessageGroupId;
         }
      }
      public List<SimulatorMessage> simulatorMessageList;

      public SimulatorModel()
      {
         simulatorMessageList = new List<SimulatorMessage>();
      }
   }
}