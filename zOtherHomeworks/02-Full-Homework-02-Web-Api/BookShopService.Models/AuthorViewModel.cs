using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class AuthorViewModel
    {
        public AuthorViewModel(Author author)
        {
            this.Id = author.Id;
            this.FirstName = author.FirstName;
            this.LastName = author.LastName;
            this.Books = new List<BookSimpleViewModel>();
            foreach (var book in author.Books)
            {
                this.Books.Add(new BookSimpleViewModel(book));
            }
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual IList<BookSimpleViewModel> Books { get; set; }
    }
}
