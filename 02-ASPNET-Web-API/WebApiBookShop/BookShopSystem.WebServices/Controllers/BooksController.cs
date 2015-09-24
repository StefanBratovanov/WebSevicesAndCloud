using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using BookShopSystem.Data;
using BookShopSystem.Models;
using BookShopSystem.WebServices.Models.BindingModels;
using BookShopSystem.WebServices.Models.ViewModels;
using System.Web.OData;

namespace BookShopSystem.WebServices.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        public IHttpActionResult GetBookById(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.NotFound();
            }

            BookFullViewModel bookInfo = new BookFullViewModel(book);

            return this.Ok(bookInfo);
        }


        //GET api/books?title={word}
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult SearchBook([FromUri]BookSearchBindingModel model)
        {
            var bookSearchResult = context.Books.AsQueryable();

            if (model.Title != null)
            {
                bookSearchResult = bookSearchResult
                    .OrderBy(b => b.Title)
                    .Where(b => b.Title.Contains(model.Title))
                    .Take(10);
            }

            var result = bookSearchResult.Select(b => new
            {
                b.Title,
                b.Id
            });

            return this.Ok(result.ToList());
        }

        // PUT /api/books/{id}
        [HttpPut]
        public IHttpActionResult EditBook(int id, [FromUri]BookBindingModel model)
        {
            var book = context.Books.Find(id);

            if (book == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Model is empty!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            book.Title = model.Title;
            book.Description = model.Description;
            book.EditionType = model.EditionType;
            book.Price = model.Price;
            book.Copies = model.Copies;
            book.ReleaseDate = model.ReleaseDate;
            book.AgeRestriction = model.AgeRestriction;
            book.AuthorId = model.AuthorId;

            context.SaveChanges();

            var returnData = context.Books
                .Where(b => b.Id == book.Id)
                .Select(b => new
                {
                    b.Title,
                    b.Description,
                    b.EditionType,
                    b.Price,
                    b.Copies,
                    b.ReleaseDate,
                    b.AgeRestriction,
                    b.AuthorId
                })
                .FirstOrDefault();

            return this.Ok(returnData);

        }

        //DELETE	/api/books/{id}

        public IHttpActionResult DeleteBookById(int id)
        {
            var book = context.Books.Find(id);

            if (book == null)
            {
                return this.NotFound();
            }

            context.Books.Remove(book);
            context.SaveChanges();

            return this.Ok();
        }

        // POST: api/books
        [HttpPost]
        public IHttpActionResult PostBook([FromBody]BookPostBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var categories = model.Categories.Split(' ').Select(c => c.Trim()).ToList();
            var categoriesList = new List<Category>();

            foreach (var cat in categories)
            {
                var databaseCategory = context.Categories.FirstOrDefault(c => c.Name == cat);
                if (databaseCategory == null)
                {
                    categoriesList.Add(new Category() { Name = cat });
                }
                else
                {
                    categoriesList.Add(databaseCategory);
                }
            }

            var bookToPost = new Book()
            {
                Title = model.Title,
                Description = model.Description,
                EditionType = model.EditionType,
                Price = model.Price,
                Copies = model.Copies,
                ReleaseDate = model.ReleaseDate,
                AgeRestriction = model.AgeRestriction,
                AuthorId = model.AuthorId,
                Categories = categoriesList
            };

            context.Books.AddOrUpdate(bookToPost);
            context.SaveChanges();

            return this.Ok(bookToPost);
        }

        // PUT /api/books/buy/{id}

        [Authorize]
        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult PurchaseBook(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.NotFound();
            }

            if (book.Copies <= 0)
            {
                return this.BadRequest("No available books left!");
            }

            book.Copies--;

            var purchase = new Purchase()
            {
                Price = book.Price,
                DateOfPurchase = DateTime.Now,
                BookId = book.Id,
                ApplicationUser = context.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name),
                IsRecalled = false
            };

            context.Purchases.Add(purchase);
            context.SaveChanges();

            var data = new PurchaseViewModel(purchase);

            return this.Ok(data);
        }

        [Authorize]
        [Route("Recall/{id}")]
        public IHttpActionResult PutRecallBook(int id)
        {
            var purchase = context.Purchases.FirstOrDefault(p => p.Id == id);
            if (purchase == null)
            {
                return this.NotFound();
            }

            if (purchase.ApplicationUser.UserName != this.User.Identity.Name)
            {
                return this.BadRequest("You cannot return this purchase, you are not the buyer!");
            }

            purchase.IsRecalled = true;
            purchase.Book.Copies++;
            context.SaveChanges();

            return this.Ok("Refunded!");
        }

    }
}

