using CurrencyUpdateService.CurrencyService;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities;

namespace CurrencyUpdateService
{
    internal sealed class DolarBancoCentralInterface
    {
        #region Public Methods

        public IEnumerable<BccrVentanillaModel> GetTodaysBccrVentanillaModel()
        {
            var exchangeUrlSource = ConfigurationManager.AppSettings["DollarColonesUrl"];
            return GetTodaysBccrVentanillaModel(exchangeUrlSource);
        }

        public IEnumerable<BccrVentanillaModel> GetTodaysBccrVentanillaModel(string urlSource)
        {
            if (string.IsNullOrEmpty(urlSource))
                throw new ArgumentException("Cannot be null or empty", urlSource);
            var exchangeUrlSource = urlSource;
            var rateMinStr = ConfigurationManager.AppSettings["DollarColonesMinValue"];
            var rateMaxStr = ConfigurationManager.AppSettings["DollarColonesMaxValue"];
            var dateMinStr = ConfigurationManager.AppSettings["ExchangeMinDate"];
            var dateMaxStr = ConfigurationManager.AppSettings["ExchangeMaxDate"];
            var formatDate = ConfigurationManager.AppSettings["SiteFormatDate"];

            var rateMin = StringUtilities.ConvertToDouble(rateMinStr);
            var rateMax = StringUtilities.ConvertToDouble(rateMaxStr);
            var dateMin = ParseDateTime(dateMinStr, formatDate);
            var dateMax = ParseDateTime(dateMaxStr, formatDate);

            var stringRows = GetRowTableValuesFromUrl(exchangeUrlSource);
            var exchangeRateList = stringRows.Select(item =>
                 GetRowTableFixedValues(item, exchangeUrlSource, rateMin, rateMax, dateMin, dateMax, formatDate))
                .Where(item => item != null);
            return exchangeRateList;
        }

        public IEnumerable<BccrVentanillaModel> GetTodaysBccrVentanillaModelFromStringDoc(string htmlDocument)
        {
            var rateMinStr = ConfigurationManager.AppSettings["DollarColonesMinValue"];
            var rateMaxStr = ConfigurationManager.AppSettings["DollarColonesMaxValue"];
            var dateMinStr = ConfigurationManager.AppSettings["ExchangeMinDate"];
            var dateMaxStr = ConfigurationManager.AppSettings["ExchangeMaxDate"];
            var formatDate = ConfigurationManager.AppSettings["SiteFormatDate"];

            var rateMin = StringUtilities.ConvertToDouble(rateMinStr);
            var rateMax = StringUtilities.ConvertToDouble(rateMaxStr);
            var dateMin = ParseDateTime(dateMinStr, formatDate);
            var dateMax = ParseDateTime(dateMaxStr, formatDate);

            var stringRows = GetRowTableValuesFromStringHtmlDocument(htmlDocument);
            var exchangeRateList = stringRows.Select(item =>
                 GetRowTableFixedValues(item, htmlDocument, rateMin, rateMax, dateMin, dateMax, formatDate))
                .Where(item => item != null);
            return exchangeRateList;
        }

        #endregion

        #region Private Methods

        private static BccrVentanillaModel GetRowTableFixedValues(string[] rowValues, string url, double rateMin,
                                        double rateMax, DateTime dateMin, DateTime dateMax, string formatDate)
        {
            try
            {
                if (rowValues == null || rowValues.Length < 4)
                {
                    throw new InvalidExchangeRateCreationArgException("all", "invalid for all values");
                }
                var bank = rowValues[0];
                if (string.IsNullOrEmpty(bank))
                    throw new InvalidExchangeRateCreationArgException("bank");

                var purchase = GetFloat(rowValues[1]);
                if (purchase > rateMax || purchase < rateMin)
                    throw new InvalidExchangeRateCreationArgException("purchase");

                var sell = GetFloat(rowValues[2]);
                if (sell > rateMax || sell < rateMin)
                    throw new InvalidExchangeRateCreationArgException("sell");

                DateTime lastUpdate = ParseDateTime(rowValues[3], formatDate);
                if (lastUpdate > dateMax || lastUpdate < dateMin)
                    throw new InvalidExchangeRateCreationArgException("lastUpdate");
                return new BccrVentanillaModel
                {
                    EntityName = bank,
                    LastUpdate = lastUpdate,
                    Purchase = purchase,
                    Sell = sell
                };
            }
            catch (InvalidExchangeRateCreationArgException)
            {
                return null;
            }

        }

        private static float GetFloat(string value)
        {
            if (value == null)
                return 0;
            value = value.Replace(',', '.');
            float result;
            result = float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? result : 0;
            return result;
        }

        private static string[] GetRowTableValues(string bankName, string url)
        {
            if (string.IsNullOrEmpty(bankName))
                throw new ArgumentException("bankName");
            var htmlDocument = WebClientUtilities.GetHtmlDocumentByUrl(url);
            var table = htmlDocument.GetElementbyId("DG");
            var rows = table.ChildNodes;
            foreach (HtmlNode row in rows)
            {
                var columns = row.ChildNodes;
                if (columns.Count <= 1)
                    continue;
                var banco = GetHmtlNodeInnerText(columns, 2);
                if (!banco.ToUpper().Contains(bankName.ToUpper()))
                    continue;
                var compra = GetHmtlNodeInnerText(columns, 3);
                var venta = GetHmtlNodeInnerText(columns, 4);
                var ultimaActualizacion = GetHmtlNodeInnerText(columns, 6);
                return new[]
                    {
                        banco, compra, venta, ultimaActualizacion
                    };
            }
            return new string[] { };
        }

        private static IEnumerable<string[]> GetRowTableValuesFromUrl(string url)
        {
            var htmlDocument = WebClientUtilities.GetHtmlDocumentByUrl(url);
            return GetRowTableValuesFromHtmlDocument(htmlDocument);
        }

        private static IEnumerable<string[]> GetRowTableValuesFromHtmlDocument(HtmlDocument htmlDocument)
        {
            var table = htmlDocument.GetElementbyId("DG");//TODO move to config file
            var rows = table.ChildNodes;
            var list = new List<string[]>();
            foreach (HtmlNode row in rows)
            {
                var columns = row.ChildNodes;
                if (columns.Count <= 1)
                    continue;
                var banco = GetHmtlNodeInnerText(columns, 2);

                var compra = GetHmtlNodeInnerText(columns, 3);
                var venta = GetHmtlNodeInnerText(columns, 4);
                var ultimaActualizacion = GetHmtlNodeInnerText(columns, 6);
                var stringRow = new[]
                    {
                        banco, compra, venta, ultimaActualizacion
                    };
                list.Add(stringRow);
            }
            return list;
        }

        private static IEnumerable<string[]> GetRowTableValuesFromStringHtmlDocument(string stringHtmlDocument)
        {
            var htmlDocument = WebClientUtilities.CreateHtmlDocument(stringHtmlDocument);
            return GetRowTableValuesFromHtmlDocument(htmlDocument);
        }

        private static string GetHmtlNodeInnerText(HtmlNodeCollection nodeCollection, int index)
        {
            if (nodeCollection == null || index >= nodeCollection.Count)
                return null;
            var value = nodeCollection[index].InnerText ?? "";
            return value.Trim();
        }

        private static DateTime ParseDateTime(string stringDate, string formatDate)
        {
            if (string.IsNullOrEmpty(stringDate))
                return new DateTime();

            const RegexOptions options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);

            stringDate = Regex.Replace(stringDate, @"\p{Z}", " ");
            formatDate = Regex.Replace(formatDate, @"\p{Z}", " ");

            stringDate = regex.Replace(stringDate, " ");
            formatDate = regex.Replace(formatDate, " ");

            stringDate = stringDate.Replace("p.m", "pm");
            stringDate = stringDate.Replace("a.m", "am");

            DateTime result;
            return DateTime.TryParseExact(stringDate, formatDate, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out result)
                       ? result
                       : new DateTime();
        }

        #endregion
    }
}
