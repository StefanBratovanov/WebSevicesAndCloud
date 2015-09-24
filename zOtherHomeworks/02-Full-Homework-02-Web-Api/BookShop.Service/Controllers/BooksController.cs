using BookShop.Data;
using BookShop.Models;
using BookShop.Service.Models;
using BookShop.Service.Providers;
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
    public class BooksController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // Example: GET api/books/1
        [HttpGet]
        [ResponseType(typeof(BookDetailedViewModel))]
        public IHttpActionResult GetBookById(int id)
        {
            Book currentBook = context.Books.Where(b => b.Id == id).FirstOrDefault();

            var bookDetailedView = new BookDetailedViewModel(currentBook);
            
            if (bookDetailedView == null)
            {
                return NotFound();
            }
            return Ok(bookDetailedView);
        }

        // Example: GET api/books?search={word}
        [HttpGet]
        [ResponseType(typeof(BookSimpleViewModel))]
        [EnableQuery]
        public IHttpActionResult SearchBooksByGivenWord(string search)
        {
            var books = context.Books
                .Where(b => b.Title.Contains(search))
                .OrderBy(b => b.Title)
                .Take(10);
            if (books.Count() == 0)
            {
                return this.NotFound();
            }

            var wantedBooks = new List<BookSimpleViewModel>();
            foreach (var book in books)
            {
                wantedBooks.Add(new BookSimpleViewModel(book));
            }
            
            return this.Ok(wantedBooks);
        }

        // Example: PUT api/books/{id}
        [HttpPut]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult EditBook(int id, [FromBody]BookBindingModel newBookDetails)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.NotFound();
            }
            book.Title = newBookDetails.Title;
            book.Desciption = newBookDetails.Desciption;
            book.Edition = newBookDetails.Edition;
            book.Price = newBookDetails.Price;
            book.Copies = newBookDetails.Copies;
            book.ReleaseDate = newBookDetails.ReleaseDate;
            book.AuthorId = newBookDetails.AuthorId;

            context.SaveChanges();

            return this.Ok("Book has been edited.");
        }

        // Example: POST api/books
        [HttpPost]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult CreateBook([FromBody]BookBindingModel bindingBook)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = new Book() 
            {
                Title = bindingBook.Title,
                Edition = bindingBook.Edition,
                Price = bindingBook.Price,
                Copies = bindingBook.Copies,
                AuthorId = bindingBook.AuthorId
            };
            foreach (var item in bindingBook.Categories)
            {
                book.Categories.Add(new Category() { Name = item.Name});
            }

            context.SaveChanges();

            return this.Ok(book);
        }

        // Example: DELETE api/books/{id}
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult DeleteBook(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("No such book.");
            }
            context.Books.Remove(book);
            context.SaveChanges();

            return this.Ok("Book has been deleted.");
        }

        // Buy a book
        // Example: PUT api/books/buy/{id}
        [Authorize]
        [HttpPut]
        [Route("api/books/buy/{id}")]
        public IHttpActionResult BuyBook(int id)
        {
            var book = context.Books.Where(b => b.Id == id).FirstOrDefault();
            if (book == null)
            {
                return this.BadRequest("No such book.");
            }

            var bookCopies = book.Copies;
            if (bookCopies == 0)
            {
                return this.BadRequest("No copies left.");
            }

            book.Copies--;

            var currentUserName = this.User.Identity.Name;
            var currentUser = context.Users
                .Where(u => u.UserName == currentUserName)
                .FirstOrDefault();
            
            var purchase = new Purchase()
            {
                Price = book.Price,
                DateOfPurchase = DateTime.Now,
                BookId = book.Id,
                UserId = currentUser.Id
            };

            context.Purchases.Add(purchase);

            currentUser.Purchases.Add(purchase);

            context.SaveChanges();

            return this.Ok("You have bought a book: " + book.Title);
        }

        // Return bought book
        // Example: PUT api/books/recall/{id}
        [Authorize]
        [HttpPut]
        [Route("api/books/recall/{id}")]
        public IHttpActionResult ReturnABook(int id)
        {
            var book = context.Books.Where(b => b.Id == id).FirstOrDefault();
            if (book == null)
            {
                return this.BadRequest("No such book.");
            }

            var currentUserName = this.User.Identity.Name;
            var currentUser = context.Users
                .Where(u => u.UserName == currentUserName)
                .FirstOrDefault();
           
            var currentUserBoughtBooks = currentUser.Purchases.Select(b => b.Book.Title).ToList();
            if (!currentUserBoughtBooks.Contains(book.Title))
            {
                return this.BadRequest("You do not bought that book.");
            }

            var currentPurchase = context.Purchases
                .Where(p => p.BookId == book.Id)
                .FirstOrDefault();
            if (currentPurchase.IsRecalled == true)
            {
                return this.BadRequest("You have already returned that book.");
            }


            var daysSinceBoughtABook = (DateTime.Now - currentPurchase.DateOfPurchase).TotalDays;
            if (daysSinceBoughtABook < 30)
            {
                currentUser.Purchases.Remove(currentPurchase);
                book.Copies++;
                currentPurchase.IsRecalled = true;
                currentUser.Purchases.Add(currentPurchase);
            }

            context.SaveChanges();

            return this.Ok("You have succecfully returned book " + book.Title);
        }
    }
}
