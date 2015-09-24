using BookShop.Data;
using BookShop.Models;
using BookShopService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;

namespace BookShop.Service.Controllers
{
    public class AuthorsController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // Example: GET api/authors/{id}
        [HttpGet]
        [ResponseType(typeof(AuthorViewModel))]
        public IHttpActionResult GetAuthorById(int id)
        {
            var author = context.Authors
                .Where(a => a.Id == id)
                .FirstOrDefault();
            if (author == null)
            {
                return this.NotFound();
            }
           
            return this.Ok(new AuthorViewModel(author));
        }

        // Example: GET api/authors/{id}/books
        [HttpGet]
        [Route("api/authors/{id}/books")]
        [ResponseType(typeof(BookDetailedViewModel))]
        [EnableQuery]
        public IHttpActionResult GetAuthorBooks(int id)
        {
            var authorBooks = context.Books
                .Where(b => b.AuthorId == id);
            var books = new List<BookDetailedViewModel>();
            foreach (var authorBook in authorBooks)
            {

                books.Add(new BookDetailedViewModel((Book)authorBook));
            }

            return this.Ok(books);
        }

        // Example: POST api/authors
        // in reques body: "FirstName = {null}, LastName={lastName}"
        [HttpPost]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult CreateAuthor([FromBody]AuthorBindingModel model)
        {
            var authorsLastNames = context.Authors.Select(a => a.LastName).ToList();
            if (authorsLastNames.Contains(model.LastName))
            {
                return this.BadRequest("Author has already existed.");
            }
            var newAuthor = new Author() 
            {
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            context.Authors.Add(newAuthor);
            context.SaveChanges();

            return this.Ok("Author has been created.");
        }
    }
}
