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
    public class TechcrunchJson
    {
        public DateTime datePublished { get; set; }
    }
    public class Techcrunch : WebSite
    {
        public Techcrunch(ApplicationDbContext context) : base(context)
        {
            this.Name = "techcrunch";
            SetRootUrl();
        }
        public  String[] GetLinks2()
        {
            
            var html = RootUrl;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var links = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class,'post-block__title__link')]");




            String[] tags = new String[links.Count];
            for (int i=0;i<links.Count;i++)
            {
                tags[i] = links[i].Attributes["href"].Value;


            }
            return tags;
        }
        public override List<string> GetLinks()
        {
            var html = RootUrl;
            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionReadEncoding = false;
            var request = (HttpWebRequest)WebRequest.Create(html);
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    htmlDoc.Load(stream, Encoding.UTF8);
                }
            }


            var links = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class,'post-block__title__link')]");
            
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
            String[] links = GetLinks2();
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
                        string metaname = item.GetAttributeValue("name", "");
                        string metaproperty = item.GetAttributeValue("property", "");
                        if (metaproperty == "og:url")
                        {
                            Url = WebUtility.HtmlDecode(item.GetAttributeValue("content", ""));
                        }
                        if (metaname == "sailthru.title")
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
                    BbcJson myJson = JsonConvert.DeserializeObject<BbcJson>(json);
                    ReleaseDate = myJson.datePublished;


                    AddDb();
                }

            }
        }
    }
}