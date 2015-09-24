using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class AuthorViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int AuthorId { get; set; }

        public ICollection<BookTitleViewModel> Books { get; set; }

        public AuthorViewModel(Author author)
        {
            FirstName = author.FirstName;
            LastName = author.LastName;
            AuthorId = author.Id;

            Books = new List<BookTitleViewModel>();

            foreach (var book in author.Books)
            {
                Books.Add(new BookTitleViewModel(book));
            }
        }


    }
}