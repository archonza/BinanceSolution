using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models
{
   public class UserInputModel
   {
      public decimal EntrancePrice { get; set; }
      public decimal ProfitTargetPrice { get; set; }
      public decimal StopLossPrice { get; set; }
      public decimal Quantity { get; set; }
      public bool TrailingProfitTarget { get; set; }
      public bool TrailingStopLoss { get; set; }
      public ETradeOrderType TradeOrderType { get; set; }
      public decimal EntranceTolerance { get; set; }
      public decimal ProfitTargetTolerance { get; set; }
      public decimal StopLossTolerance { get; set; }
   }
}