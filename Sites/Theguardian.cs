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
    public class Theguardian : WebSite
    {
        public Theguardian(ApplicationDbContext context) : base(context)
        {
            this.Name = "theguardian";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'fc-item__content ')]//a[1]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }

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

                var html = link;
                if (!IfExists(html))
                {
                    HtmlWeb web = new HtmlWeb();
                    web.OverrideEncoding = Encoding.UTF8;
                    var htmlDoc = web.Load(html);
                    var list = htmlDoc.DocumentNode.SelectNodes("//meta");

                    foreach (var item in list)
                    {
                        string metaproperty = item.GetAttributeValue("property", "");
                        string metaitemprop = item.GetAttributeValue("itemprop", "");

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
                        if (metaproperty == "article:published_time")
                        {
                            ReleaseDate = Convert.ToDateTime(WebUtility.HtmlDecode(item.GetAttributeValue("content", "")));
                        }

                    }
                    if (Content == "")
                    {
                          var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content__standfirst')]//p[1]");
                          Content = WebUtility.HtmlDecode(node.InnerText);
                    }

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