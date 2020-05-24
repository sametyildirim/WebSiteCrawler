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
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Name;
        public abstract List<string> GetLinks();
        public abstract void Crawl();
        public ApplicationDbContext _context;
        
        public WebSite(ApplicationDbContext context)
        {
            this._context = context;
        }
        public void AddDb()
        {
            var existingArticle = _context.Articles.FirstOrDefault(a => a.Url == this.Url);
            if (existingArticle == null)
            {
                var articleSource = _context.Sources.FirstOrDefault(a => a.Name == this.Name);
                Article article = new Article();
                article.Url = this.Url;
                article.Subject = this.Subject;
                article.Content = this.Content;
                article.IsActive = true;
                article.IsDeleted = false;
                article.CreateDate = DateTime.Today;
                if(this.Image!=null) this.Image = this.Image.Replace("http:","https:");
                article.ImgUrl = this.Image;
                article.ReleaseDate = this.ReleaseDate;
                article.ArticleSource  = articleSource;

                _context.Articles.Add(article);
                _context.SaveChanges();
            }
        }

    }
}