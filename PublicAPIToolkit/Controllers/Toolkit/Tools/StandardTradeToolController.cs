﻿using PublicAPIToolkit.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using static PublicAPIToolkit.Models.InfoModel;
using PublicAPIToolkit.Models.InputModels.Toolkit.Tools;
using PublicAPIToolkit.Models.DomainModels.Trade;
using PublicAPIToolkit.Models.ViewModels.Toolkit.Tools;
using PublicAPIToolkit.Models.DomainModels.Exchange;
using System.Threading;

namespace PublicAPIToolkit.Controllers.Toolkit.Tools
{
   public class StandardTradeToolController : Controller
   {
      public static TradeDataModel exchangeDataModel = new TradeDataModel();
      public static InfoController infoController = new InfoController(@"C:\Users\za120317\Test.txt");
      public TradeOrderController tradeOrderController = new TradeOrderController();
      private RestClientController restClientController;
      private StandardTradeToolViewModel standardTradeToolViewModel = new StandardTradeToolViewModel();
      Thread tradeTread;
      private Object threadLock = new Object();

      public StandardTradeToolController()
      {
      }

      // GET: DataAnalysisTool
      public ActionResult Index()
      {
         return View();
      }

      [HttpPost]
      public JsonResult Refresh(StandardTradeToolInputModel standardTradeToolInputModel)
      {
         restClientController = new RestClientController(standardTradeToolInputModel.EndPoint, standardTradeToolInputModel.HttpMethod);
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
         standardTradeToolViewModel.CurrencyPair = currencyPair.ToString();
         standardTradeToolViewModel.Amount = amount;
         exchangeDataModel.TickerPrice = amount;

         return Json(standardTradeToolViewModel, JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      public string SubmitUserInput(StandardTradeToolInputModel standardTradeToolInputModel)
      {
         TradeOrder tradeOrder = new TradeOrder();
         tradeOrder.CurrencyPair = (ECurrencyPair)standardTradeToolInputModel.CurrencyPair;
         tradeOrder.EntrancePrice = standardTradeToolInputModel.EntrancePrice;
         tradeOrder.StopLossPrice = standardTradeToolInputModel.StopLossPrice;
         tradeOrder.ProfitTargetPrice = standardTradeToolInputModel.ProfitTargetPrice;
         tradeOrder.Quantity = standardTradeToolInputModel.Quantity;
         tradeOrder.TrailingProfitTarget = standardTradeToolInputModel.TrailingProfitTarget;
         tradeOrder.TrailingStopLoss = standardTradeToolInputModel.TrailingStopLoss;
         tradeOrder.TradeOrderType = (ETradeOrderType)standardTradeToolInputModel.TradeOrderType;
         tradeOrder.EntranceTolerance = standardTradeToolInputModel.EntranceTolerance;
         tradeOrder.ProfitTargetTolerance = standardTradeToolInputModel.ProfitTargetTolerance;
         tradeOrder.StopLossTolerance = standardTradeToolInputModel.StopLossTolerance;

         tradeOrderController.AddTradeOrder(tradeOrder);
         tradeOrder.TradeState = ETradeState.UNINITIALISED;
         lock (threadLock)
         {
            /* Add info messages */
            infoController.AddInfo(
               true,
               EInfoMessageDescriptor.EntryGreaterThanTicker,
               "Entry price is greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.EntryLessThanTicker,
               "Entry price is less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.DetectEntrance,
               "Detecting trade entrance." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.DetectExit,
               "Detecting trade exit." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.BuyOrderEnteredEntryGreaterThanTicker,
               "Trade was entered (BUY ORDER) with entry originally greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.BuyOrderEnteredEntryLessThanTicker,
               "Trade was entered (BUY ORDER) with entry originally less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.SellOrderEnteredEntryGreaterThanTicker,
               "Trade was entered (SELL ORDER) with entry originally greater than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.SellOrderEnteredEntryLessThanTicker,
               "Trade was entered (SELL ORDER) with entry originally less than ticker." + " Entrance price: " + tradeOrder.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.ExitedWithProfit,
               "Trade was exited with a profit." + " Target profit price: " + tradeOrder.ProfitTargetPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
            infoController.AddInfo(
               false,
               EInfoMessageDescriptor.ExitedWithLoss,
               "Trade was exited with a loss." + " Stop loss price: " + tradeOrder.StopLossPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now);
         }
         tradeTread = new Thread(() => InitiateTrade(tradeOrder));

         /* Start new tade thread */
         tradeTread.Start();

         return "Hello from http post web api controller: ";
      }

      private static void InitiateTrade(TradeOrder tradeOrder)
      {
         while (tradeOrder.TradeState != ETradeState.EXIT)
         {
            /* Is trade uninitialised */
            if (tradeOrder.TradeState == ETradeState.UNINITIALISED)
            {
               /* If so, initialise */
               InitialiseTrade(tradeOrder);
            }

            /* Is trade inactive */
            if (tradeOrder.TradeState == ETradeState.INACTIVE)
            {
               /* If so, detect entrance */
               DetectTradeEntrance(tradeOrder);
            }
            /* Is trade active */
            else if (tradeOrder.TradeState == ETradeState.ACTIVE)
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

      private static void InitialiseTrade(TradeOrder tradeOrder)
      {
         if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.Undefined)
         {
            if (tradeOrder.EntrancePrice > exchangeDataModel.TickerPrice)
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryGreaterThanTicker;
               infoController.PrintAllGroups(EInfoMessageDescriptor.EntryGreaterThanTicker);
            }
            else
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryLessThanTicker;
               infoController.PrintAllGroups(EInfoMessageDescriptor.EntryLessThanTicker);
            }
         }
         tradeOrder.TradeState = ETradeState.INACTIVE;
      }

      private static void DetectTradeEntrance(TradeOrder tradeOrder)
      {
         infoController.PrintAllGroups(EInfoMessageDescriptor.DetectEntrance);
         /*
          When this function is called for the first time, determine if when it is a buy order, whether 
          entrance price is greater that or less that ticker price in order to determine whether tolerance
          should be set to be greater than entrance price or less than entrance price.
          */
         if (tradeOrder.TradeOrderType == ETradeOrderType.BUY)
         {
            DetectBuyOrderTradeEntrance(tradeOrder);
         }
         else if (tradeOrder.TradeOrderType == ETradeOrderType.SELL)
         {
            DetectSellOrderTradeEntrance(tradeOrder);
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectTradeExit(TradeOrder tradeOrder)
      {
         infoController.PrintAllGroups(EInfoMessageDescriptor.DetectExit);
         DetectProfitTarget(tradeOrder);
         DetectStopLoss(tradeOrder);
      }

      private static void DetectBuyOrderTradeEntrance(TradeOrder tradeOrder)
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
               infoController.PrintAllGroups(EInfoMessageDescriptor.BuyOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = ETradeState.INACTIVE;
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
               infoController.PrintAllGroups(EInfoMessageDescriptor.BuyOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = ETradeState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectSellOrderTradeEntrance(TradeOrder tradeOrder)
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
               infoController.PrintAllGroups(EInfoMessageDescriptor.SellOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = ETradeState.ACTIVE;
               tradeOrder.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = ETradeState.INACTIVE;
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
               infoController.PrintAllGroups(EInfoMessageDescriptor.SellOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               tradeOrder.TradeState = ETradeState.ACTIVE;
               tradeOrder.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               tradeOrder.TradeState = ETradeState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private static void DetectProfitTarget(TradeOrder tradeOrder)
      {
         if (tradeOrder.TradeState == ETradeState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice >= tradeOrder.ProfitTargetPrice)
            {
               tradeOrder.TradeState = ETradeState.EXIT;
               infoController.PrintAllGroups(EInfoMessageDescriptor.ExitedWithProfit);
            }
         }
      }

      private static void DetectStopLoss(TradeOrder tradeOrder)
      {
         if (tradeOrder.TradeState == ETradeState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice <= tradeOrder.StopLossPrice)
            {
               tradeOrder.TradeState = ETradeState.EXIT;
               infoController.PrintAllGroups(EInfoMessageDescriptor.ExitedWithLoss);
            }
         }
      }
   }
}