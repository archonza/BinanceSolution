using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using PublicAPIToolkit.Simulator.Controllers;
using PublicAPIToolkit.Simulator.Models;
using static PublicAPIToolkit.Simulator.Models.SimulatorModel;
using PublicAPIToolkit.Toolkit.Models;
using PublicAPIToolkit.Exchange.Controllers;
using PublicAPIToolkit.Exchange.Models;
using PublicAPIToolkit.Rest.Controllers;
using System.Threading;

namespace PublicAPIToolkit.Controllers.Toolkit.Tools
{
   public class TradeToolController : Controller
   {
      public static TradeDataModel exchangeDataModel = new TradeDataModel();
      public static SimulatorController simulatorController = new SimulatorController(@"C:\Users\za120317\Test.txt");
      public ExchangeOrderController tradeOrderController = new ExchangeOrderController();
      private RestClientController restClientController;
      private TradeToolViewModel tradeToolViewModel = new TradeToolViewModel();
      Thread tradeTread;
      private Object threadLock = new Object();

      public TradeToolController()
      {
      }

      // GET: DataAnalysisTool
      public ActionResult Index()
      {
         return View();
      }

      [HttpPost]
      public JsonResult Refresh(TradeToolInputModel tradeToolInputModel)
      {
         restClientController = new RestClientController(tradeToolInputModel.EndPoint, tradeToolInputModel.HttpMethod);
         restClientController.MakeRequest();
         //ECurrencyPair currencyPair = new ECurrencyPair();
         //decimal tickerPrice = 0.0M;

         //exchangeDataModel.CurrencyPair = currencyPair;
         //exchangeDataModel.TickerPrice = tickerPrice;
         //InitiateTrade();
         string rawTicker = restClientController.GetResponse();
         ECurrencyPair currencyPair = ECurrencyPair.NOT_SELECTED;
         decimal amount = 0.0m;
         ExchangeController.ProcessRawData(rawTicker, ref currencyPair, ref amount);
         tradeToolViewModel.CurrencyPair = currencyPair.ToString();
         tradeToolViewModel.Amount = amount;
         exchangeDataModel.TickerPrice = amount;

         return Json(tradeToolViewModel, JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      public string SubmitUserInput(TradeToolInputModel tradeToolInputModel)
      {
         ExchangeOrder tradeOrder = new ExchangeOrder();
         tradeOrder.CurrencyPair = (ECurrencyPair)tradeToolInputModel.CurrencyPair;
         tradeOrder.EntrancePrice = tradeToolInputModel.EntrancePrice;
         tradeOrder.StopLossPrice = tradeToolInputModel.StopLossPrice;
         tradeOrder.ProfitTargetPrice = tradeToolInputModel.ProfitTargetPrice;
         tradeOrder.Quantity = tradeToolInputModel.Quantity;
         tradeOrder.TrailingProfitTarget = tradeToolInputModel.TrailingProfitTarget;
         tradeOrder.TrailingStopLoss = tradeToolInputModel.TrailingStopLoss;
         tradeOrder.TradeOrderType = (EExchangeOrderType)tradeToolInputModel.TradeOrderType;
         tradeOrder.EntranceTolerance = tradeToolInputModel.EntranceTolerance;
         tradeOrder.ProfitTargetTolerance = tradeToolInputModel.ProfitTargetTolerance;
         tradeOrder.StopLossTolerance = tradeToolInputModel.StopLossTolerance;

         tradeOrderController.AddTradeOrder(tradeOrder);
         tradeOrder.TradeState = EExchangeOrderState.UNINITIALISED;
         lock (threadLock)
         {
            /* Add simulator messages */
            simulatorController.AddSimulator(
               true,
               ESimulatorMessageDescriptor.EntryGreaterThanTicker,
               "Entry price is greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.EntryLessThanTicker,
               "Entry price is less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.DetectEntrance,
               "Detecting trade entrance." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.DetectExit,
               "Detecting trade exit." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.BuyOrderEnteredEntryGreaterThanTicker,
               "Trade was entered (BUY ORDER) with entry originally greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.BuyOrderEnteredEntryLessThanTicker,
               "Trade was entered (BUY ORDER) with entry originally less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.SellOrderEnteredEntryGreaterThanTicker,
               "Trade was entered (SELL ORDER) with entry originally greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.SellOrderEnteredEntryLessThanTicker,
               "Trade was entered (SELL ORDER) with entry originally less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.ExitedWithProfit,
               "Trade was exited with a profit." + " Target profit price: " + tradeOrder.ProfitTargetPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            simulatorController.AddSimulator(
               false,
               ESimulatorMessageDescriptor.ExitedWithLoss,
               "Trade was exited with a loss." + " Stop loss price: " + tradeOrder.StopLossPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
         }
         tradeTread = new Thread(() => InitiateTrade(tradeOrder));

         /* Start new tade thread */
         tradeTread.Start();

         return "Hello from http post web api controller: ";
      }

      private static void InitiateTrade(ExchangeOrder tradeOrder)
      {
         while (tradeOrder.TradeState != EExchangeOrderState.EXIT)
         {
            /* Is trade uninitialised */
            if (tradeOrder.TradeState == EExchangeOrderState.UNINITIALISED)
            {
               /* If so, initialise */
               InitialiseTrade(tradeOrder);
            }

            /* Is trade inactive */
            if (tradeOrder.TradeState == EExchangeOrderState.INACTIVE)
            {
               /* If so, detect entrance */
               DetectTradeEntrance(tradeOrder);
            }
            /* Is trade active */
            else if (tradeOrder.TradeState == EExchangeOrderState.ACTIVE)
            {
               /* If so, detect exit */
               DetectTradeExit(tradeOrder);
            }
            else
            {
               Debug.WriteLine("ERROR: Invalid trade state detected.");
            }
         }
      }

      private static void InitialiseTrade(ExchangeOrder tradeOrder)
      {
         if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.Undefined)
         {
            if (tradeOrder.EntrancePrice > exchangeDataModel.TickerPrice)
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryGreaterThanTicker;
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.EntryGreaterThanTicker);
            }
            else
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryLessThanTicker;
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.EntryLessThanTicker);
            }
         }
         tradeOrder.TradeState = EExchangeOrderState.INACTIVE;
      }

      private static void DetectTradeEntrance(ExchangeOrder tradeOrder)
      {
         simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.DetectEntrance);
         /*
          When this function is called for the first time, determine if when it is a buy order, whether 
          entrance price is greater that or less that ticker price in order to determine whether tolerance
          should be set to be greater than entrance price or less than entrance price.
          */
         if (tradeOrder.TradeOrderType == EExchangeOrderType.BUY)
         {
            DetectBuyOrderTradeEntrance(tradeOrder);
         }
         else if (tradeOrder.TradeOrderType == EExchangeOrderType.SELL)
         {
            DetectSellOrderTradeEntrance(tradeOrder);
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectTradeExit(ExchangeOrder tradeOrder)
      {
         simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.DetectExit);
         DetectProfitTarget(tradeOrder);
         DetectStopLoss(tradeOrder);
      }

      private static void DetectBuyOrderTradeEntrance(ExchangeOrder tradeOrder)
      {
         /* 
          * E.g. I want to buy 1000 units at 10.00 
          * The ticker price is 5.00
          * I want to wait until the price is between 10.00 and (10.00 + tolarence)
          */
         /* Check whether entrance price is greater that initial ticker price */
         if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.EntryGreaterThanTicker)
         {
            /* If so, determine whether trade should be entered */
            if ((exchangeDataModel.TickerPrice >= tradeOrder.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (tradeOrder.EntrancePrice +
                 tradeOrder.EntranceTolerance)))
            {
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.BuyOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = EExchangeOrderState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = EExchangeOrderState.INACTIVE;
            }
         }
         /* 
          * E.g. I want to buy 1000 units at 10.00 
          * The ticker price is 15.00
          * I want to wait until the price is between 10.00 and ((10.00 * 0.1) / 100) + 10.00 = 10.0010
          */
         /* Check whether entrance price is less that initial ticker price */
         else if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.EntryLessThanTicker)
         {
            /* If so, determine whether trade should be entered */
            if ((exchangeDataModel.TickerPrice >= tradeOrder.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (tradeOrder.EntrancePrice +
                 tradeOrder.EntranceTolerance)))
            {
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.BuyOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = EExchangeOrderState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = EExchangeOrderState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectSellOrderTradeEntrance(ExchangeOrder tradeOrder)
      {
         /* 
          * E.g. I want to sell 1000 units at 10.00 
          * The ticker price is 5.00
          * I want to wait until the price is between 10.00 - ((10.00 * 0.1) / 100) = 9.9910 and 10.00
          */
         /* Check whether entrance price is greater that initial ticker price */
         if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.EntryGreaterThanTicker)
         {
            /* If so, determine whether trade should be entered */
            if ((exchangeDataModel.TickerPrice >= (tradeOrder.EntrancePrice -
                 tradeOrder.EntranceTolerance)) &&
                (exchangeDataModel.TickerPrice <= tradeOrder.EntrancePrice))
            {
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.SellOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = EExchangeOrderState.ACTIVE;
               tradeOrder.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = EExchangeOrderState.INACTIVE;
            }
         }
         /* 
          * E.g. I want to buy 1000 units at 10.00 
          * The ticker price is 15.00
          * I want to wait until the price is between 10.00 and ((10.00 * 0.1) / 100) + 10.00 = 10.0010
          */
         /* Check whether entrance price is less that initial ticker price */
         else if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.EntryLessThanTicker)
         {
            /* If so, determine whether trade should be entered */
            if ((exchangeDataModel.TickerPrice >= tradeOrder.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (tradeOrder.EntrancePrice + tradeOrder.EntranceTolerance)))
            {
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.SellOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = EExchangeOrderState.ACTIVE;
               tradeOrder.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = EExchangeOrderState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectProfitTarget(ExchangeOrder tradeOrder)
      {
         if (tradeOrder.TradeState == EExchangeOrderState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice >= tradeOrder.ProfitTargetPrice)
            {
               tradeOrder.TradeState = EExchangeOrderState.EXIT;
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.ExitedWithProfit);
            }
         }
      }

      private static void DetectStopLoss(ExchangeOrder tradeOrder)
      {
         if (tradeOrder.TradeState == EExchangeOrderState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice <= tradeOrder.StopLossPrice)
            {
               tradeOrder.TradeState = EExchangeOrderState.EXIT;
               simulatorController.PrintAllGroups(ESimulatorMessageDescriptor.ExitedWithLoss);
            }
         }
      }
   }
}