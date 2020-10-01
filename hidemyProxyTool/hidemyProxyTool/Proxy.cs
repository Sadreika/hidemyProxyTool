using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace hidemyProxyTool
{
    public class Proxy
    {
        private string proxyAddress { get; set; }
        private string port { get; set; }
        public Proxy(string proxyAddress, string port)
        {
            this.proxyAddress = proxyAddress;
            this.port = port;
        }
        public Proxy()
        {

        }
        public List<Proxy> hidemyProxy()
        {
            string url = "https://hidemy.name/en/proxy-list/?type=h#list";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
            //request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Host = "hidemy.name";
            request.ContentType = "keep-alive";
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            var response = (HttpWebResponse)(request.GetResponse());
            List<Proxy> proxyAddressList = new List<Proxy>();
            List<String> allCollectedData = new List<string>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                htmlDocument.Load(streamReader);

                foreach (HtmlNode proxyAddress in htmlDocument.DocumentNode.SelectNodes("//tr//td"))
                {
                    allCollectedData.Add(proxyAddress.InnerText);
                }

                string queryForIPAddress = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
                Regex regex = new Regex(queryForIPAddress);
                for (int i = 0; i < allCollectedData.Count; i++)
                {
                    if (regex.IsMatch(allCollectedData[i]))
                    {
                        if (Int16.Parse(allCollectedData[i + 3].Remove(allCollectedData[i + 3].Length - 3, 3)) < 1000)
                        {
                            Proxy proxy = new Proxy(allCollectedData[i], allCollectedData[i + 1]);
                            proxyAddressList.Add(proxy);
                            i = i + 1;
                        }
                    }
                }
            }
            return proxyAddressList;
        }
    }
}
