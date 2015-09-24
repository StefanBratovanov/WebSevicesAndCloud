namespace News.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using News.Models;
    using News.Repositories;
    using News.Services.Models;

    public class NewsController : BaseApiController
    {
        public NewsController()
        {
        }

        public NewsController(IRepository<News> data)
            : base(data)
        {
        }

        [HttpGet]
        public IHttpActionResult GetAllNews()
        {
            var news = this.Data.All()
                .OrderByDescending(n => n.PublishedDate)
                .Select(NewsViewModel.CreateView);

            return this.Ok(news);
        }

        [HttpPost]
        public IHttpActionResult CreateNews(NewsBindingModel newsDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (newsDto == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var news = new News
                           {
                               Title = newsDto.Title, 
                               Content = newsDto.Content, 
                               PublishedDate = newsDto.PublishedDate ?? DateTime.Now
                           };
            this.Data.Add(news);
            this.Data.SaveChanges();

            return this.CreatedAtRoute("DefaultApi", new { id = news.Id }, news);
        }

        [HttpPut]
        public IHttpActionResult EditNews(int id, NewsBindingModel newsDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (newsDto == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var news = this.Data.Find(id);

            if (news == null)
            {
                return this.NotFound();
            }

            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.PublishedDate = newsDto.PublishedDate;
            this.Data.SaveChanges();

            return this.Ok(new NewsViewModel(news));
        }

        [HttpDelete]
        public IHttpActionResult DeleteNews(int id)
        {
            var news = this.Data.Find(id);

            if (news == null)
            {
                return this.NotFound();
            }

            this.Data.Delete(news);
            this.Data.SaveChanges();

            return this.Ok();
        }
    }
}