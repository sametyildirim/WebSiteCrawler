using System.Collections.Generic;
using System;
using BoilerPlateCms.Data;
using System.Linq;

namespace WebSiteCrawler
{
    public abstract class WebSite
    {
        public string Url { get; set; }
        public string Subject { get; set; }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set 
            {
                if(value.Length>250)
                {
                    int lastDotIndex = value.Substring(250,value.Length-250).IndexOf(".");
                    _content = value.Substring(0,250+lastDotIndex+1);
                }
                else
                {
                    _content = value;
                }
            }
        }
        public string Image { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Name { get; set; }

        public string RootUrl { get; set; }
        public abstract List<string> GetLinks();
        public abstract void Crawl();
        public ApplicationDbContext _context;

        public WebSite(ApplicationDbContext context)
        {
            this._context = context;
        }
        public void SetRootUrl()
        {
            var source = _context.Sources.FirstOrDefault(a => a.Name == this.Name);
            this.RootUrl = source.Url;
        }
        public bool IfExists(string _url)
        {
            var existingArticle = _context.Articles.FirstOrDefault(a => a.Url == _url);
            if (existingArticle != null)
            {
                return true;
            }
            return false;
        }
        public void AddDb()
        {
            var existingArticle = _context.Articles.FirstOrDefault(a => a.Url == this.Url);

            var articleSource = _context.Sources.FirstOrDefault(a => a.Name == this.Name);
            Article article = new Article();
            article.Url = this.Url;
            article.Subject = this.Subject;
            article.Content = this.Content;
            article.IsActive = true;
            article.IsDeleted = false;
            article.CreateDate = DateTime.Today;
            if (this.Image != null) this.Image = this.Image.Replace("http:", "https:");
            article.ImgUrl = this.Image;
            article.ReleaseDate = this.ReleaseDate;
            article.Source = articleSource;

            _context.Articles.Add(article);
            _context.SaveChanges();

        }

    }
}