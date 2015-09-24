using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class BookCategoryViewModel
    {
        public string Category { get; set; }

        public BookCategoryViewModel(Category category)
        {
            Category = category.Name;
        }
    }
}