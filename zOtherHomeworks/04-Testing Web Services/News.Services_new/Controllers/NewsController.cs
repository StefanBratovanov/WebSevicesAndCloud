using System;
using System.Linq;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using News.Data;
using News.Services_new.Controllers;
using News.Services_new.Infrastructure;
using News.Services_new.Models;

namespace News.Services.Controllers
{
    [RoutePrefix("api/news")]
    public class NewsController : BaseApiController
    {
        public NewsController()
            : base()
        {
        }

        public NewsController(INewsData data, IUserIdProvider userIdProvider)
            : base(data, userIdProvider)
        {
        }

        public IHttpActionResult GetAllNews()
        {
            var news = this.Data.News.All()
                .OrderByDescending(n => n.PublishDate)
                .Select(NewsViewModel.Create);
            return this.Ok(news);
        }



        [Authorize]
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateNews(CreateNewsBindingModel model)
        {
            var loggedUsedId = this.UserIdProvider.GetUserId();
            if (loggedUsedId == null)
            {
                return this.BadRequest("Please log in the system to post news");
            }
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            if (model.Title==null || model.Content==null)
            {
                return this.BadRequest();
            }
            var loggedUser = this.Data.Users.Find(loggedUsedId);
            var news = new News.Models.News()
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = DateTime.Now,
                Author = loggedUser
            };
            this.Data.News.Add(news);
            this.Data.SaveChanges();
            var result = this.Data.News.Find(news.Id);
            return this.Ok(result);

        }


        [Route("{id}")]
        public IHttpActionResult PutNews(CreateNewsBindingModel model, int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            if (model.Title.IsNullOrWhiteSpace() || model.Content.IsNullOrWhiteSpace())
            {
                return this.BadRequest();
            }
            var news = this.Data.News.Find(id);
            if (news == null)
            {
                return this.BadRequest("News does not exist");
            }
            news.Title = model.Title;
            news.Content = model.Content;
            news.PublishDate = DateTime.Now;
            this.Data.SaveChanges();
            var result = this.Data.News.Find(id);
            return this.Ok(result);
        }
        [Route("{id}")]
        public IHttpActionResult DeleteNews(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            var news = this.Data.News.Find(id);
            if (news == null)
            {
                return this.BadRequest("News does not exist");
            }
            this.Data.News.Delete(news);
            this.Data.SaveChanges();
            return this.Ok();
        }
    }
}
