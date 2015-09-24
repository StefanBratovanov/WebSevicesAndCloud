using System.Web.Http;
using News.Data;
using News.Services_new.Infrastructure;

namespace News.Services_new.Controllers
{
    public class BaseApiController : ApiController
    {
        public BaseApiController() :
            this(new NewsData(new NewsContext()), new AspNetUserIdProvider())
        {

        }

        public BaseApiController(INewsData data, IUserIdProvider userIdProvider)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvider;
        }

        protected INewsData Data { get; set; }

        protected IUserIdProvider UserIdProvider { get; set; }
    }
}
