using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class BookSimpleViewModel
    {
        public BookSimpleViewModel(Book book)
        {
            this.Id = book.Id;
            this.Title = book.Title;
        }

        public int Id { get; set; }

        public string Title { get; set; }
    }
}
