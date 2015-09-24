using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using BookShopSystem.Data;
using BookShopSystem.Models;
using BookShopSystem.WebServices.Models.BindingModels;
using BookShopSystem.WebServices.Models.ViewModels;

namespace BookShopSystem.WebServices.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {

        private BookShopContext context = new BookShopContext();

        // GET: api/authors/{id}
        public IHttpActionResult GetAuthorsById(int id)
        {
            var author = context.Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return this.NotFound();
            }

            AuthorViewModel authorInfo = new AuthorViewModel(author);

            return this.Ok(authorInfo);
        }

        [Route("{id}/books")]
        public IHttpActionResult GetAuthorsBooksById(int id)
        {
            var author = context.Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return this.NotFound();
            }

            AuthorsBooksViewModel authorInfo = new AuthorsBooksViewModel(author);

            return this.Ok(authorInfo);
        }


        // POST: api/Authors
        [HttpPost]
        public IHttpActionResult PostAuthor([FromBody]AddAuthorBindingModel authorModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var authorToAdd = new Author()
            {
                FirstName = authorModel.FirstName,
                LastName = authorModel.LastName
            };

            context.Authors.AddOrUpdate(authorToAdd);

            context.SaveChanges();

            return this.Ok(authorToAdd);
        }


    }
}