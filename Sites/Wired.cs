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
    public class Wired : WebSite
    {
        public Wired(ApplicationDbContext context):base (context)
        {
            this.Name = "wired.com";
        }
        public override  List<string> GetLinks()
        {
            var html = "https://www.wired.com/tag/artificial-intelligence/";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'primary-grid-component')]/div");
            var links = node.SelectNodes("//a[contains(@href, '/story')]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if ((link.Attributes["href"].Value).StartsWith("/story") && !tags.Contains(link.Attributes["href"].Value) && link.Attributes["class"] == null)
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

                var html = "https://www.wired.com" + link;
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
                var node = htmlDoc.DocumentNode.SelectSingleNode("//time");
                ReleaseDate = Convert.ToDateTime(node.InnerText.ToString());

                AddDb();


            }
        }
    }
}