using PublicAPIToolkit.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Exchange.Controllers
{
   public class ExchangeOrderController
   {
      private List<ExchangeOrder> tradeOrderList = new List<ExchangeOrder>();

      public ExchangeOrderController()
      {
         tradeOrderList = new List<ExchangeOrder>();
      }

      public void AddTradeOrder(ExchangeOrder tradeOrder)
      {
         tradeOrderList.Add(tradeOrder);
      }
   }
}