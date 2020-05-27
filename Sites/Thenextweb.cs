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
    public class ThenextwebJson
    {
        public Image image {get;set; }
    }
    public class Image
    {
        public string url { get; set;}
    }
    public class Thenextweb : WebSite
    {
        public Thenextweb(ApplicationDbContext context) : base(context)
        {
            this.Name = "thenextweb";
            SetRootUrl();
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//h2[contains(@class,'cover-title')]/a[1]");
            List<string> tags = new List<string>();
            foreach (var link in links)
            {
                if (!tags.Contains(link.Attributes["href"].Value))
                {
                    tags.Add(link.Attributes["href"].Value);
                }

            }
            var links2 = htmlDoc.DocumentNode.SelectNodes("//h4[contains(@class,'story-title')]/a[1]");
            foreach (var link in links2)
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
            foreach (string link in links)
            {

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
                    
                    var json = WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode("//script[contains(@type, 'application/ld+json')][2]").InnerText);
                    try
                    {
                        ThenextwebJson myJson = JsonConvert.DeserializeObject<ThenextwebJson>(json);
                        Image = myJson.image!=null ? myJson.image.url : Image;
                    }
                    catch
                    {
                        var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'post-featuredImage u-m-1_5')]/img[1]");
                        Image =  node.GetAttributeValue("src", "");
                    }

                    AddDb();
                }

            }
        }
    }
}