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
    public class Theverge : WebSite
    {
        public Theverge(ApplicationDbContext context) : base(context)
        {
            this.Name = "theverge";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            //var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'primary-grid-component')]/div");
            var links = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'c-entry-box--compact__image-wrapper')]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if ( !tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }


            }
            return tags;
        }
        public override void Crawl()
        {
            List<string> links = GetLinks();
            foreach (string link in links)
            {

                var html = link;
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
                        if (content == "sailthru.date")
                        {
                            ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(item.GetAttributeValue("content", "")));
                        }
                    }

                    var json = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//script[contains(@type, 'application/ld+json')]").InnerText);
                    ContextJson myJson = JsonConvert.DeserializeObject<ContextJson>(json);
                    ReleaseDate = myJson.datePublished;


                    AddDb();
                }

            }
        }
    }
}