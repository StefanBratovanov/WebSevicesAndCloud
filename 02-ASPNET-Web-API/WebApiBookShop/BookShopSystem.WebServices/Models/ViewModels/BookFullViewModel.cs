using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class BookFullViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Age AgeRestriction { get; set; }

        public EditionType EditionType { get; set; }

        public ICollection<BookCategoryViewModel> BookCategories { get; set; }

        public string Author { get; set; }

        public int AuthorId { get; set; }

        public BookFullViewModel(Book book)
        {
            Title = book.Title;
            Description = book.Description;
            Price = book.Price;
            Copies = book.Copies;
            ReleaseDate = book.ReleaseDate;
            AgeRestriction = book.AgeRestriction;
            EditionType = book.EditionType;

            BookCategories = new List<BookCategoryViewModel>();

            foreach (var category in book.Categories)
            {
                BookCategories.Add(new BookCategoryViewModel(category));
            }
            Author = book.Author.FirstName + " " + book.Author.LastName;

            AuthorId = book.AuthorId;
        }
    }
}