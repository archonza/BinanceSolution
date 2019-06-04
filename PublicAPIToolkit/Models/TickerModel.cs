using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PublicAPIToolkit.Models
{
   public class TickerModel
   {
      public int Id { get; set; }
      public string RawData { get; set; }

      public static void ProcessRawData(string rawData, ref ECurrencyPair outCurrencyPair, ref decimal outCurrency)
      {
         /* {"symbol":"BTCUSDT","price":"3870.03000000"} */
         // Define regular expression pattern
         string pattern;
         //pattern = @"""symbol"":""(\s+)"",""price"":""(\s+)""";
         pattern = @"\{""symbol"":""(\w+)"",""price"":""(\d*\.\d*)""\}";
         Regex rx = new Regex(pattern);

         MatchCollection matchCollection = Regex.Matches(rawData, pattern);
         foreach (Match match in matchCollection)
         {
            Enum.TryParse(match.Groups[1].Value, out outCurrencyPair);
            outCurrency = Convert.ToDecimal(match.Groups[2].Value);
         }

         // linear regression (a form of polinomial regression)
         // polinomial regression
         // logistic regression (binary and catogory answers)
         // super logistic regression is nural networks
         // matlab/archave
         // standform mit machine learning course
         // know linear algebra
         // The google ledger

         //outSymbol = 
      }
   }
}