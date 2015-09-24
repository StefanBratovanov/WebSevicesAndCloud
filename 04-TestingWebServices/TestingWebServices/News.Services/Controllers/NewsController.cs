using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using News.Data.Repositories;
using News.Models;
using News.Services.Models;

namespace News.Services.Controllers
{
    // [RoutePrefix("api/News")]
    public class NewsController : BaseApiController
    {
        private IRepository<NewsRecord> repo;

        public NewsController()
        {
            this.repo = this.NewsRepository;
        }

        public NewsController(IRepository<NewsRecord> repository)
        {
            this.repo = repository;
        }

        // GET : api/news
        [HttpGet]
        [Route("api/News")]
        public IHttpActionResult GetAll()
        {
            var news = this.repo
                .GetAll()
                .OrderBy(n => n.PublishDate)
                .AsQueryable();

            return this.Ok(news);
        }

        [HttpPost]
        [Route("api/News")]
        public IHttpActionResult PostNews([FromBody]NewsRecordBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("News cannot be null!");
            }
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var newsRecord = new NewsRecord
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = DateTime.Now
            };

            repo.Add(newsRecord);
            repo.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = newsRecord.Id }, newsRecord);
            //  return this.Ok(newsRecord);
        }

        // PUT: api/News?id=5
        [HttpPut]
        [Route("api/News")]
        public IHttpActionResult PutNewsRecord(int id, NewsRecordBindingModel model)
        {
            var newsRecord = repo.GetAll().FirstOrDefault(n => n.Id == id);

            if (newsRecord == null)
            {
                return this.NotFound();
            }

            newsRecord.Title = model.Title;
            newsRecord.Content = model.Content;
            newsRecord.PublishDate = DateTime.Now;

            repo.Update(newsRecord);
            repo.SaveChanges();

            return this.Ok(newsRecord);
        }

        [HttpDelete]
        [Route("api/News/{id}")]
        public IHttpActionResult DeleteNewsRecord(int id)
        {
            var newsRecord = repo.GetAll().FirstOrDefault(n => n.Id == id);

            if (newsRecord == null)
            {
                return this.NotFound();
            }

            repo.Delete(newsRecord);
            repo.SaveChanges();

            return this.Ok(newsRecord);
        }

    }
}

// public IHttpActionResult PutNewsRecord(int id, [FromBody] NewsRecordBindingModel model)



/*
 [HttpPut]
        [Route("api/News")]
        public IHttpActionResult PutNewsRecord(int id, NewsRecordBindingModel model)
        {
            var newsRecord = repo.GetAll().FirstOrDefault(n => n.Id == id);

            if (newsRecord == null)
            {
                return this.NotFound();
            }

            newsRecord.Title = model.Title;
            newsRecord.Content = model.Content;
            newsRecord.PublishDate = DateTime.Now;

            repo.Update(newsRecord);
            repo.SaveChanges();

            return this.Ok(newsRecord);
        }
*/