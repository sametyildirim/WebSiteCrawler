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
     public class EngadgetJson
    {
        public string datePublished { get; set; }
    }
    public class Engadget : WebSite
    {
        public Engadget(ApplicationDbContext context) : base(context)
        {
            this.Name = "engadget";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            //var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'primary-grid-component')]/div");
            var links = htmlDoc.DocumentNode.SelectNodes("//article/a[1]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }


            }
            var lastlink = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'o-rating_thumb@m-')]/a");
            if (!tags.Contains(lastlink.Attributes["href"].Value))
            {
                tags.Add(lastlink.Attributes["href"].Value);
            }
            return tags;
        }
        public override void Crawl()
        {
            List<string> links = GetLinks();
            foreach (string link in links)
            {

                var html = "https://www.engadget.com" + link;
                if (!IfExists(html))
                {
                    HtmlWeb web = new HtmlWeb();
                    web.OverrideEncoding = Encoding.UTF8;
                    var htmlDoc = web.Load(html);
                    var list = htmlDoc.DocumentNode.SelectNodes("//meta");

                    foreach (var item in list)
                    {
                        string metaname = item.GetAttributeValue("name", "");
                        string metaproperty = item.GetAttributeValue("property", "");
                        if (metaproperty == "og:url")
                        {
                            Url = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (metaproperty == "og:title")
                        {
                            Subject = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (metaproperty == "og:description")
                        {
                            Content = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (metaproperty == "og:image")
                        {
                            Image = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                    }
                    
                    var json = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//script[contains(@type, 'application/ld+json')]").InnerText);
                    
                    var strinReleaseDate = json.Substring(json.IndexOf("datePublished")+17,25);
                    ReleaseDate = Convert.ToDateTime(strinReleaseDate);


                    AddDb();
                }

            }
        }
    }
}