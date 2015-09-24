using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class BookTitleViewModel
    {
        public string BookTitle { get; set; }

        public BookTitleViewModel(Book book)
        {
            BookTitle = book.Title;
        }
    }
}
