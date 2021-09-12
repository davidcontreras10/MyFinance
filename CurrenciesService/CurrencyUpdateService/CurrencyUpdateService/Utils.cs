using CurrencyUpdateService.CurrencyService;
using HtmlAgilityPack;
using System.Net.Http;

namespace CurrencyUpdateService
{
    public static class WebClientUtilities
    {
        public static HtmlDocument CreateHtmlDocument(string htmlString)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            return htmlDocument;
        }

        public static string GetHttpResponseMessage(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidBankUrlException(url, "status code: " + response.StatusCode);
                }
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static HtmlDocument GetHtmlDocumentByUrl(string url)
        {
            string htmlString = GetHttpResponseMessage(url);
            return CreateHtmlDocument(htmlString);
        }

        public static string GetHmtlNodeInnerText(HtmlNodeCollection nodeCollection, int index)
        {
            if (nodeCollection == null || index >= nodeCollection.Count)
                return null;
            var value = nodeCollection[index].InnerText ?? "";
            return value.Trim();
        }
    }
}
