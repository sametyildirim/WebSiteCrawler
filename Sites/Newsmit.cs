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
    public class Newsmit : WebSite
    {
        public Newsmit(ApplicationDbContext context):base (context)
        {
            this.Name = "newsmit";
        }
        public override  List<string> GetLinks()
        {
            var html = "https://news.mit.edu/topic/artificial-intelligence2";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'article-cover-image')]//a");
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

                var html = "https://news.mit.edu" + link;
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

                ReleaseDate = Convert.ToDateTime(htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'datePublished')]").Attributes["content"].Value);

                AddDb();


            }
        }
    }
}