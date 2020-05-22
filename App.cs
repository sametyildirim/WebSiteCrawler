using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Collections.Generic;
using HtmlAgilityPack;
using BoilerPlateCms.Data;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using WebSiteCrawler.Sites;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace WebSiteCrawler
{
    public class App
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
         private readonly IOptions<EmailSettingsModel> _emailSettings;  
        public App(IConfiguration config, ApplicationDbContext context, IOptions<EmailSettingsModel> emailSettings)
        {
            _config = config;
            _context = context;
            _emailSettings = emailSettings;
        }

        public void Run()
        {


            WebSite site = new Wired(_context);
            CrawlSite(site);

            site = new Sciencedaily(_context);
            CrawlSite(site);

            site = new Sciencenews(_context);
            CrawlSite(site);

            site = new Bbc(_context);
            CrawlSite(site);

            site = new Independent(_context);
            CrawlSite(site);

            site = new Aidaily(_context);
            CrawlSite(site);

            site = new Newsmit(_context);
            CrawlSite(site);           


        }
        public void CrawlSite(WebSite website)
        {
            try
            {
                website.Crawl();
                Console.WriteLine(website.Name + " finished");
            }
            catch (Exception ex)
            {
                SendMail(website.Name + " has error." + ex.ToString(), website.Name);
            }

        }
        public void SendMail(string mailbody, string sitename)
        {
         
            string subject = "aidailynews error " + sitename;
            string body = mailbody;
            _emailSettings.Value.SendMail(subject, body);
            
        }

    }
}