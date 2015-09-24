namespace News.Services.Controllers
{
    using System.Web.Http;

    using News.Data;
    using News.Models;
    using News.Repositories;

    public class BaseApiController : ApiController
    {
        private IRepository<News> data;

        public BaseApiController() :
            this(new NewsRepository(new NewsContext()))
        {
        }

        public BaseApiController(IRepository<News> data)
        {
            this.data = data;
        }

        protected IRepository<News> Data
        {
            get
            {
                return this.data;
            }
        } 
    }
}