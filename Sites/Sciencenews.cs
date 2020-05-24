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
    public class Sciencenews : WebSite
    {
        public Sciencenews(ApplicationDbContext context) : base(context)
        {
            this.Name = "sciencenews";
        }
        public override List<string> GetLinks()
        {
            var html = "https://www.sciencenews.org/topic/artificial-intelligence";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//article//a[1]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {

                if ((link.Attributes["href"].Value).Contains("sciencenews.org/article") && !tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }
            }

            links = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class, 'post-item-river__wrapper___2c_E- with-image')]//a[1]");
            foreach (var link in links)
            {
                if ((link.Attributes["href"].Value).Contains("sciencenews.org/article") && !tags.Contains(link.Attributes["href"].Value))
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
                    if (content == "article:published_time")
                    {
                        ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(item.GetAttributeValue("content", "")));
                    }
                }

                AddDb();

            }
        }
    }
}