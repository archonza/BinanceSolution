using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models
{
   public enum EInfoMessageDescriptor
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