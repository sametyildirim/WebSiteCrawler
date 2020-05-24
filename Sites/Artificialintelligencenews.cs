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
    public class Artificialintelligencenews : WebSite
    {
        public Artificialintelligencenews(ApplicationDbContext context):base (context)
        {
            this.Name = "artificialintelligence-news";
        }
        public override  List<string> GetLinks()
        {
            var html = "https://artificialintelligence-news.com/";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//main");
            var links = node.SelectNodes("//article//div[contains(@class, 'image')]//a");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {

                tags.Add(link.Attributes["href"].Value);
            }
            return tags;
        }
        public override void Crawl()
        {
            List<string> links = GetLinks();
            foreach (string link in links)
            {

                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(link);
                var list = htmlDoc.DocumentNode.SelectNodes("//meta");
                
                foreach (var item in list)
                {
                    string content = item.GetAttributeValue("property", "");
                    if (content == "og:url")
                    {
                        Url = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                    }
                    // if (content == "og:title")
                    // {
                    //     Subject = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                    // }
                    // if (content == "og:description")
                    // {
                    //     Content = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                    // }
                    if (content == "og:image")
                    {
                        Image = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                    }
                    if (content == "article:published_time")
                    {
                        ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(item.GetAttributeValue("content", "")));
                    }
                }
                Subject = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//header[contains(@class, 'article-header')]//h1").InnerText);
                Content = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//section[contains(@class, 'entry-content')]//p[1]").InnerText);
                AddDb();


            }
        }
    }
}