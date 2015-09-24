using System.Linq;
using System.Web.Http;
using BookShop.Data;
using BookShop.Models;
using BookShopService.Models.BindingModels;
using BookShopService.Models.ViewModels;

namespace BookShopService.Controllers
{
    public class AuthorsController : ApiController
    {
        private readonly BookShopContext _context = new BookShopContext();

        [HttpGet]
        public IHttpActionResult Get([FromUri] int id)
        {
            var author = _context.Authors
                .Select(a => new AuthorViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Books = _context.Books
                        .Where(b => b.AuthorId == id)
                        .Select(b => b.Title)
                })
                .FirstOrDefault(a => a.Id == id);

            return Ok(author);
        }

        [HttpPost]
        public IHttpActionResult PostAuthor([FromBody] AuthorBindingModel author)
        {
            if (ModelState.IsValid && author != null)
            {
                _context.Authors.Add(new Author
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });

                _context.SaveChanges();

                return Ok(author);
            }

            return BadRequest("Something went wrong");
        }

        [HttpGet]
        [Route("api/authors/{id}/books")]
        public IHttpActionResult GetAuthorBooks(int id)
        {
            var books = _context.Books
                .Where(b => b.AuthorId == id)
                .Select(b => new AuthorBooksViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    EditionType = b.EditionType,
                    AgeRestriction = b.AgeRestriction,
                    Price = b.Price,
                    Copies = b.Copies,
                    ReleaseDate = b.ReleaseDate,
                    Categories = b.Categories.Select(c => c.Name)
                });

            return Ok(books);
        }
    }
}