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
    public class Sciencedaily : WebSite
    {
        public Sciencedaily(ApplicationDbContext context) : base(context)
        {
            this.Name = "sciencedaily";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'heroes')]//a");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                    tags.Add(link.Attributes["href"].Value);
            }
            links = htmlDoc.DocumentNode.SelectNodes("//ul[contains(@id, 'featured_shorts')]//a");
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                    tags.Add(link.Attributes["href"].Value);
            }
            links = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'summaries')]//a[1]");
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

                var html = "https://www.sciencedaily.com" + link;
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
                            Url = html;
                        }
                        // if (content == "og:title")
                        // {
                        //     Subject = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        // }
                        if (content == "og:description")
                        {
                            Content = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }

                    }

                    Subject = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'headline')]").InnerText);
                    ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//dd[contains(@id, 'date_posted')]").InnerText));

                    AddDb();
                }


            }
        }
    }
}