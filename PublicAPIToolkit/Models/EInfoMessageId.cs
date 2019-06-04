using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models
{
   public enum EInfoMessageId
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