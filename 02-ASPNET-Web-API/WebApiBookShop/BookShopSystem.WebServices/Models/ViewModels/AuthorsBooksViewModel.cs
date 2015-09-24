using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class AuthorsBooksViewModel
    {
        public int AuthorId { get; set; }

        public ICollection<BookViewModel> AuthorBooks { get; set; }

        public AuthorsBooksViewModel(Author author)
        {
            AuthorId = author.Id;

            AuthorBooks = new List<BookViewModel>();

            foreach (var book in author.Books)
            {
                AuthorBooks.Add(new BookViewModel(book));
            }
        }

    }
}