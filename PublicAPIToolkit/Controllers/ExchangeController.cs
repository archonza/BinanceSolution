using PublicAPIToolkit.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PublicAPIToolkit.Controllers
{
   public class ExchangeController
   {
      public int Id { get; set; }
      public string RawData { get; set; }

      public static void ProcessRawData(string rawData, ref ECurrencyPair outCurrencyPair, ref decimal outCurrency)
      {
         /* {"symbol":"BTCUSDT","price":"3870.03000000"} */
         // Define regular expression pattern
         string pattern;
         pattern = @"\{""symbol"":""(\w+)"",""price"":""(\d*\.\d*)""\}";
         Regex rx = new Regex(pattern);

         MatchCollection matchCollection = Regex.Matches(rawData, pattern);
         foreach (Match match in matchCollection)
         {
            Enum.TryParse(match.Groups[1].Value, out outCurrencyPair);
            outCurrency = Convert.ToDecimal(match.Groups[2].Value, CultureInfo.InvariantCulture);
         }
      }
   }
}