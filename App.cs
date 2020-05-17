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
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Sciencedaily(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Sciencenews(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Bbc(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Independent(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Aidaily(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");

            site = new Newsmit(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");
            
            site = new Artificialintelligencenews(_context);
            site.Crawl();
            Console.WriteLine(site.Name +" finished");



        }

    }
}