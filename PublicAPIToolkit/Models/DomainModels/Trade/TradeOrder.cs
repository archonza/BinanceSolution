using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Models.DomainModels.Trade
{
   public class TradeOrder
   {
      private static int NextId { get; set; } = -1;

      public int Id { get; private set; } = -1;
      public ECurrencyPair CurrencyPair { get; set; }
      public decimal EntrancePrice { get; set; }
      public decimal ProfitTargetPrice { get; set; }
      public decimal StopLossPrice { get; set; }
      public DateTime? EntranceDateTime { get; set; } = null;
      public DateTime? ExitDateTime { get; set; } = null;
      public decimal Quantity { get; set; }
      public ETradeState TradeState { get; set; } = ETradeState.UNINITIALISED;
      public bool TrailingProfitTarget { get; set; }
      public bool TrailingStopLoss { get; set; }
      public ETradeOrderType TradeOrderType { get; set; }
      public decimal EntranceTolerance { get; set; }
      public decimal ProfitTargetTolerance { get; set; }
      public decimal StopLossTolerance { get; set; }

      

      public TradeOrder()
      {
         NextId++;
         Id = NextId;
      }
   }
}