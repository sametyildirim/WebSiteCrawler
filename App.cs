using Microsoft.Extensions.Configuration;
using System;
using BoilerPlateCms.Data;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WebSiteCrawler.Sites;

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

            StartAllCrawling();
            // WebSite website = new Engadget(_context);
            // website.Crawl();
            

        }
     
        public void StartAllCrawling()
        {
            using (StreamWriter w = File.AppendText("/tmp/log.txt"))
            {
                Log("Job Started", w);
            }

            var classList = typeof(WebSite).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(WebSite)) && !t.IsAbstract);
            var sourceList = _context.Sources.ToList();

            foreach (var myclass in classList)
            {

                var instantiatedObject = Activator.CreateInstance(Type.GetType(myclass.ToString()), _context);
                CrawlSite((WebSite)instantiatedObject, sourceList);
            }

            using (StreamWriter w = File.AppendText("/tmp/log.txt"))
            {
                Log("Job Finished", w);
            }
        }
        public void CrawlSite(WebSite website, List<Source> sourceList)
        {
            try
            {
                if (sourceList.Where(x => x.Name == website.Name).FirstOrDefault().IsActive)
                {
                    website.Crawl();
                    Console.WriteLine(website.Name + " finished");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("/tmp/log.txt"))
                {
                    Log("Error Occured " + ex.ToString(), w);
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