namespace Battleships.WebServices.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Data;

    public class UsersController : BaseApiController
    {
        public UsersController(IBattleshipsData data) : base(data)
        {
        }

        [HttpGet]
        public IHttpActionResult GetUsersCount()
        {
            var count = this.Data.Users.All().Count();
            return this.Ok(count);
        }

        [Route("api/users/{username}/ssn")]
        [HttpPut]
        public IHttpActionResult PutSsn(string username, SsnBinding ssnBinding)
        {
            var ctx = ApplicationDbContext.Create();
            var user = ctx.Users.FirstOrDefault(x => x.UserName == username);
            user.Ssn = ssnBinding.ssn;
            ctx.SaveChanges();
            return this.Ok();
        }
    }

    public class SsnBinding
    {
        public string ssn { get; set; }
    }
}