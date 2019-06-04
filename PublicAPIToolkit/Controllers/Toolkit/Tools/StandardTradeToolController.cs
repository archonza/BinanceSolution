using PublicAPIToolkit.Models;
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

namespace PublicAPIToolkit.Controllers.Toolkit.Tools
{
   public class StandardTradeToolController : Controller
   {
      public static TradeDataModel exchangeDataModel = new TradeDataModel();
      public InfoModel infoModel;
      public InfoController infoController;
      public TradeOrderController tradeOrderController = new TradeOrderController();
      private RestClientController restClientController;
      private StandardTradeToolViewModel standardTradeToolViewModel = new StandardTradeToolViewModel();

      public StandardTradeToolController()
      {
         infoModel = new InfoModel();
         /* Add info messages */
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.EntryGreaterThanTicker, 
            "Entry price is greater than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.EntryLessThanTicker, 
            "Entry price is less than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.DetectEntrance,
            "Detecting trade entrance." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.DetectExit,
            "Detecting trade exit." + " Ticker price: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.BuyOrderEnteredEntryGreaterThanTicker,
            "Trade was entered (BUY ORDER) with entry originally greater than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.BuyOrderEnteredEntryLessThanTicker,
            "Trade was entered (BUY ORDER) with entry originally less than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.SellOrderEnteredEntryGreaterThanTicker,
            "Trade was entered (SELL ORDER) with entry originally greater than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.SellOrderEnteredEntryLessThanTicker,
            "Trade was entered (SELL ORDER) with entry originally less than ticker." + " Entrance price: " + exchangeDataModel.EntrancePrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.ExitedWithProfit,
            "Trade was exited with a profit." + " Target profit price: " + exchangeDataModel.ProfitTargetPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoModel.infoMessageList.Add(new InfoMessage(
            EInfoMessageId.ExitedWithLoss,
            "Trade was exited with a loss." + " Stop loss price: " + exchangeDataModel.StopLossPrice + " TickerPrice: " + exchangeDataModel.TickerPrice + " Date/Time: " + DateTime.Now)
            );
         infoController = new InfoController(infoModel, @"E:\Test.txt");
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
         decimal amount = 0;
         ExchangeController.ProcessRawData(rawTicker, ref currencyPair, ref amount);
         standardTradeToolViewModel.CurrencyPair = currencyPair.ToString();
         standardTradeToolViewModel.Amount = amount;
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

         return "Hello from http post web api controller: ";
      }

      private void InitiateTrade()
      {
         /* Is trade uninitialised */
         if (exchangeDataModel.TradeState == ETradeState.UNINITIALISED)
         {
            /* If so, initialise */
            InitialiseTrade();
         }

         /* Is trade inactive */
         if (exchangeDataModel.TradeState == ETradeState.INACTIVE)
         {
            /* If so, detect entrance */
            DetectTradeEntrance();
         }
         /* Is trade active */
         else if (exchangeDataModel.TradeState == ETradeState.ACTIVE)
         {
            /* If so, detect exit */
            DetectTradeExit();
         }
         else
         {
            Debug.WriteLine("ERROR: Invalid trade state detected.");
         }
      }

      private void InitialiseTrade()
      {
         if (exchangeDataModel.EntryTickerCompare == EEntryTickerCompareType.Undefined)
         {
            if (exchangeDataModel.EntrancePrice > exchangeDataModel.TickerPrice)
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryGreaterThanTicker;
               infoController.Print(EInfoMessageId.EntryGreaterThanTicker);
            }
            else
            {
               exchangeDataModel.EntryTickerCompare = EEntryTickerCompareType.EntryLessThanTicker;
               infoController.Print(EInfoMessageId.EntryLessThanTicker);
            }
         }
         exchangeDataModel.TradeState = ETradeState.INACTIVE;
      }

      private void DetectTradeEntrance()
      {
         infoController.Print(EInfoMessageId.DetectEntrance);
         /*
          When this function is called for the first time, determine if when it is a buy order, whether 
          entrance price is greater that or less that ticker price in order to determine whether tolerance
          should be set to be greater than entrance price or less than entrance price.
          */
         if (exchangeDataModel.TradeOrderType == ETradeOrderType.BUY)
         {
            DetectBuyOrderTradeEntrance();
         }
         else if (exchangeDataModel.TradeOrderType == ETradeOrderType.SELL)
         {
            DetectSellOrderTradeEntrance();
         }
         else
         {
            /* Do nothing */
         }
      }

      private void DetectTradeExit()
      {
         infoController.Print(EInfoMessageId.DetectExit);
         DetectProfitTarget();
         DetectStopLoss();
      }

      private void DetectBuyOrderTradeEntrance()
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
            if ((exchangeDataModel.TickerPrice >= exchangeDataModel.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (exchangeDataModel.EntrancePrice +
                 exchangeDataModel.EntranceTolerance)))
            {
               infoController.Print(EInfoMessageId.BuyOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               exchangeDataModel.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
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
            if ((exchangeDataModel.TickerPrice >= exchangeDataModel.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (exchangeDataModel.EntrancePrice +
                 exchangeDataModel.EntranceTolerance)))
            {
               infoController.Print(EInfoMessageId.BuyOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               exchangeDataModel.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private void DetectSellOrderTradeEntrance()
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
            if ((exchangeDataModel.TickerPrice >= (exchangeDataModel.EntrancePrice -
                 exchangeDataModel.EntranceTolerance)) &&
                (exchangeDataModel.TickerPrice <= exchangeDataModel.EntrancePrice))
            {
               infoController.Print(EInfoMessageId.SellOrderEnteredEntryGreaterThanTicker);
               /* Enter trade */
               exchangeDataModel.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
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
            if ((exchangeDataModel.TickerPrice >= exchangeDataModel.EntrancePrice) &&
                (exchangeDataModel.TickerPrice <= (exchangeDataModel.EntrancePrice + exchangeDataModel.EntranceTolerance)))
            {
               infoController.Print(EInfoMessageId.SellOrderEnteredEntryLessThanTicker);
               /* Enter trade */
               exchangeDataModel.TradeState = ETradeState.ACTIVE;
               exchangeDataModel.EntranceDateTime = DateTime.Now;
            }
            else
            {
               /* Trade remains inactive */
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
            }
         }
         else
         {
            /* Do nothing */
         }
      }

      private void DetectProfitTarget()
      {
         if (exchangeDataModel.TradeState == ETradeState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice >= exchangeDataModel.ProfitTargetPrice)
            {
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
               infoController.Print(EInfoMessageId.ExitedWithProfit);
            }
         }
      }

      private void DetectStopLoss()
      {
         if (exchangeDataModel.TradeState == ETradeState.ACTIVE)
         {
            if (exchangeDataModel.TickerPrice <= exchangeDataModel.StopLossPrice)
            {
               exchangeDataModel.TradeState = ETradeState.INACTIVE;
               infoController.Print(EInfoMessageId.ExitedWithLoss);
            }
         }
      }
   }
}