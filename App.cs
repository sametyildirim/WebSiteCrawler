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

namespace WebSiteCrawler
{
    public class App
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public App(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
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
            var fromAddress = new MailAddress("samet52@gmail.com", "aidailynews");
            var toAddress = new MailAddress("samet52@gmail.com", "aidailynews");
            const string fromPassword = "olpqooxjqqkrgjau";
            string subject = "aidailynews error " + sitename;
            string body = mailbody;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

    }
}