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
    public class Aidaily : WebSite
    {
        public Aidaily(ApplicationDbContext context) : base(context)
        {
            this.Name = "aidaily";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'summary-title')]//a");
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
            int i = 0;
            foreach (string link in links)
            {
                i++;
                var html = "https://aidaily.co.uk" + link;
                if (!IfExists(html))
                {
                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(html);

                    var list = htmlDoc.DocumentNode.SelectNodes("//meta");
                    foreach (var item in list)
                    {
                        string content = item.GetAttributeValue("property", "");
                        string itemprop = item.GetAttributeValue("itemprop", "");
                        if (content == "og:url")
                        {
                            Url = html;
                        }
                        if (itemprop == "headline")
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
                        if (itemprop == "datePublished")
                        {
                            ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(item.GetAttributeValue("content", "")));
                        }

                    }
                    Image = Image.Replace("1500w", "300w");
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