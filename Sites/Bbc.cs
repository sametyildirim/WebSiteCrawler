using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Collections.Generic;
using HtmlAgilityPack;
using BoilerPlateCms.Data;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace WebSiteCrawler.Sites
{
    public class BbcJson
    {
        public DateTime datePublished { get; set; }
    }
    public class Bbc : WebSite
    {
        public Bbc(ApplicationDbContext context) : base(context)
        {
            this.Name = "bbc";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//article//a[1]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                    tags.Add(link.Attributes["href"].Value);
            }
            return tags;
        }
        public override void Crawl()
        {
            List<string> links = GetLinks();
            foreach (string link in links)
            {

                var html = "https://www.bbc.com" + link;
                if (!IfExists(html))
                {
                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(html);
                    var list = htmlDoc.DocumentNode.SelectNodes("//meta");

                    foreach (var item in list)
                    {
                        string content = item.GetAttributeValue("property", "");
                        if (content == "og:url")
                        {
                            Url = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (content == "og:title")
                        {
                            Subject = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (content == "og:description")
                        {
                            Content = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (content == "og:image")
                        {
                            Image = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                    }

                    var json = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//script[contains(@type, 'application/ld+json')]").InnerText);
                    BbcJson myJson = JsonConvert.DeserializeObject<BbcJson>(json);
                    ReleaseDate = myJson.datePublished;

                    AddDb();
                }


            }
        }
    }
}