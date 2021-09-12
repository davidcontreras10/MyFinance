﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace MyFinanceWebApp.Services
{
    public class WebServiceCaller
    {
        public string Url { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public XmlDocument ResultXml;
        public string ResultString;


        public WebServiceCaller(string url, string methodName)
        {
            Url = url;
            MethodName = methodName;
        }

        /// <summary>
        /// Invokes service
        /// </summary>
        public void Invoke()
        {
            Invoke(true);
        }

        /// <summary>
        /// Invokes service
        /// </summary>
        /// <param name="encode">Added parameters will encode? (default: true)</param>
        public void Invoke(bool encode)
        {
            try
            {
                string soapStr =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <{0} xmlns=""http://tempuri.org/"">
                  {1}
                </{0}>
              </soap:Body>
            </soap:Envelope>";

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                req.Headers.Add("SOAPAction", "\"http://tempuri.org/" + MethodName + "\"");
                req.ContentType = "text/xml;charset=\"utf-8\"";
                req.Accept = "text/xml";
                req.Method = "POST";


                using (Stream stm = req.GetRequestStream())
                {
                    string postValues = "";
                    foreach (var param in Params)
                    {
                        if (encode)
                            postValues += string.Format("<{0}>{1}</{0}>", HttpUtility.UrlEncode(param.Key), HttpUtility.UrlEncode(param.Value));
                        else
                            postValues += string.Format("<{0}>{1}</{0}>", param.Key, param.Value);

                    }

                    soapStr = string.Format(soapStr, MethodName, postValues);
                    using (StreamWriter stmw = new StreamWriter(stm))
                    {
                        stmw.Write(soapStr);
                    }
                }

                using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    string result = responseReader.ReadToEnd();
                    //result = HttpUtility.HtmlDecode(result);
                    ResultXml = new XmlDocument();
                    ResultXml.LoadXml(result);
                    ResultString = result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public string GetResponseXml()
        {
            string responseTagName = MethodName + "Response";
            XmlNodeList list = ResultXml.GetElementsByTagName(responseTagName);
            if (list.Count == 0)
                return "";
            XmlNode node = list[0];
            return node.InnerText;
        }
    }
}