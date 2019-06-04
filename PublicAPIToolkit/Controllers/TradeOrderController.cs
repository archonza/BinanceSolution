using PublicAPIToolkit.Models.DomainModels.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Controllers
{
   public class TradeOrderController
   {
      private List<TradeOrder> tradeOrderList = new List<TradeOrder>();

      public TradeOrderController()
      {
         tradeOrderList = new List<TradeOrder>();
      }

      public void AddTradeOrder(TradeOrder tradeOrder)
      {
         tradeOrderList.Add(tradeOrder);
      }
   }
}