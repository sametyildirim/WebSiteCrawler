using Microsoft.Extensions.Configuration;
using System;
using BoilerPlateCms.Data;
using WebSiteCrawler.Sites;
using Microsoft.Extensions.Options;
using System.IO;

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

            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log("Job Started", w);
            }

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

            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log("Job Finished", w);
            }


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
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    Log("Error Occured "+ex.ToString(), w);
                }
                SendMail(website.Name + " has error." + ex.ToString(), website.Name);
            }

        }
        public void SendMail(string mailbody, string sitename)
        {

            string subject = "aidailynews error " + sitename;
            string body = mailbody;
            _emailSettings.Value.SendMail(subject, body);

        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

    }
}