using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Simulator.Models
{
   public enum ESimulatorMessageDescriptor
   {
      EntryGreaterThanTicker,
      EntryLessThanTicker,
      DetectEntrance,
      DetectExit,
      BuyOrderEnteredEntryGreaterThanTicker,
      BuyOrderEnteredEntryLessThanTicker,
      SellOrderEnteredEntryGreaterThanTicker,
      SellOrderEnteredEntryLessThanTicker,
      ExitedWithProfit,
      ExitedWithLoss
   }
}