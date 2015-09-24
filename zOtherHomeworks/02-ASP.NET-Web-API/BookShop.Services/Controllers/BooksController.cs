using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BookShop.Data;
using BookShop.Models;
using BookShopService.Models.BindingModels;
using BookShopService.Models.ViewModels;

namespace BookShopService.Controllers
{
    [Authorize]
    public class BooksController : ApiController
    {
        private readonly BookShopContext _context = new BookShopContext();

        [HttpGet]
        public IHttpActionResult GetBookById(int id)
        {
            var book = _context.Books
                .Where(b => b.Id == id)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    EditionType = b.EditionType,
                    AgeRestriction = b.AgeRestriction,
                    Price = b.Price,
                    Copies = b.Copies,
                    ReleaseDate = b.ReleaseDate,
                    AuthorId = b.AuthorId,
                    Author = b.Author,
                    Categories = b.Categories.Select(c => c.Name)
                });

            return Ok(book);
        }

        [HttpGet]
        [Route("api/books")]
        public IHttpActionResult GetBooksByKeyWord(string search)
        {
            var books = _context.Books
                .Where(b => b.Title.Contains(search) || b.Description.Contains(search))
                .Take(10)
                .Select(b => new BooksByKeywordViewModel
                {
                    Id = b.Id,
                    Title = b.Title
                });

            return Ok(books);
        }

        [HttpPut]
        [Route("api/books/{id}")]
        public IHttpActionResult EditBook(int id, [FromBody] BookBindingModel bookModel)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            book.Title = bookModel.Title;
            book.Description = bookModel.Description;
            book.EditionType = bookModel.EditionType;
            book.AgeRestriction = bookModel.AgeRestriction;
            book.Price = bookModel.Price;
            book.Copies = bookModel.Copies;
            book.ReleaseDate = bookModel.ReleaseDate;
            book.AuthorId = bookModel.AuthorId;
            book.Author = _context.Authors.FirstOrDefault(a => a.Id == bookModel.AuthorId);

            _context.SaveChanges();

            return Ok("Book edited successfully!");
        }

        [HttpDelete]
        public IHttpActionResult DeleteBook(int id)
        {
            _context.Books.Remove(_context.Books.FirstOrDefault(b => b.Id == id));

            _context.SaveChanges();

            return Ok("Book deleted successfully!");
        }

        [HttpPost]
        [Route("api/books")]
        public IHttpActionResult PostBook(BookBindingModel bookModel)
        {
            var catNames = bookModel.Categories.Split(' ');

            ICollection<Category> categories = catNames
                .Select(name => _context.Categories.FirstOrDefault(c => c.Name == name))
                .ToList();

            _context.Books.Add(new Book
            {
                Title = bookModel.Title,
                Description = bookModel.Description,
                EditionType = bookModel.EditionType,
                AgeRestriction = bookModel.AgeRestriction,
                Price = bookModel.Price,
                Copies = bookModel.Copies,
                ReleaseDate = bookModel.ReleaseDate,
                AuthorId = bookModel.AuthorId,
                Author = _context.Authors.FirstOrDefault(a => a.Id == bookModel.AuthorId),
                Categories = categories
            });

            _context.SaveChanges();

            return Ok("Book added successfully!");
        }

        [HttpPut]
        [Route("api/books/buy/{id}")]
        public IHttpActionResult BuyBook(int id, [FromBody] BookPurchaseBindingModel purchase)
        {
            if (User.Identity == null)
                return Unauthorized();

            if (!_context.Books.Any(b => b.Id == id))
                return BadRequest("Book with this ID doesnt exist!");

            if (_context.Books.FirstOrDefault(b => b.Id == id).Copies == 0)
                return BadRequest("Book is out of stock.");

            _context.Purchases.Add(new Purchase
            {
                Book = _context.Books.FirstOrDefault(b => b.Id == id),
                DateOfPurchase = purchase.DateOfPurchase,
                IsRecalled = purchase.IsRecalled,
                Price = purchase.Price,
                User = _context.Users.FirstOrDefault(u => u.UserName == purchase.User)
            });

            _context.Configuration.ValidateOnSaveEnabled = false;

            _context.Books.FirstOrDefault(b => b.Id == id).Copies--;

            _context.SaveChanges();

            return Ok("You successfully purchased: " + _context.Books.FirstOrDefault(b => b.Id == id).Title);
        }

        [HttpPut]
        [Route("api/books/recall/{id}")]
        public IHttpActionResult RecallBook(int id)
        {
            if (User.Identity == null)
                return Unauthorized();

            if (!_context.Books.Any(b => b.Id == id))
                return BadRequest("Book with this ID doesnt exist!");

            if (!_context.Users.Any(u => u.Purchases.Count(p => p.Book.Id == id) > 0))
                return BadRequest("You dont have that book.");

            var dateNow = DateTime.Now.AddDays(-30);

            var purchase = _context.Purchases
                .Where(p => dateNow <= p.DateOfPurchase)
                .OrderBy(p => p.DateOfPurchase).First();

            _context.Purchases.Remove(purchase);

            _context.Configuration.ValidateOnSaveEnabled = false;

            _context.Books.FirstOrDefault(b => b.Id == id).Copies++;

            _context.SaveChanges();

            return Ok("You successfully recalled the book: " + _context.Books.FirstOrDefault(b => b.Id == id).Title);
        }
    }
}