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
    public class ZdnetJson
    {
        public DateTime datePublished { get; set; }
        public string headline { get; set; }
    }
    public class Zdnet : WebSite
    {
        public Zdnet(ApplicationDbContext context) : base(context)
        {
            this.Name = "zdnet";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//article//div[contains(@class, 'content')]//h3/a[1]");
            List<string> tags = new List<string>();
            int i=0;
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }
                i++;

                if(i==15) break;

            }
            return tags;
        }
        public override void Crawl()
        {
            List<string> links = GetLinks();
            int i = 0;
            foreach (string link in links)
            {
                i++;
                var html = "https://www.zdnet.com"+link;
                if (!IfExists(html))
                {
                    HtmlWeb web = new HtmlWeb();
                    web.OverrideEncoding = Encoding.UTF8;
                    var htmlDoc = web.Load(html);
                    var list = htmlDoc.DocumentNode.SelectNodes("//meta");

                    foreach (var item in list)
                    {
                        string metaproperty = item.GetAttributeValue("property", "");
                        if (metaproperty == "og:url")
                        {
                            Url = html;
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
                    ZdnetJson myJson = JsonConvert.DeserializeObject<ZdnetJson>(json);
                    ReleaseDate = myJson.datePublished;
                    Subject = myJson.headline;


                    AddDb();
                }
                 else if (i == 1)
                {
                    return;
                }

            }
        }
    }
}