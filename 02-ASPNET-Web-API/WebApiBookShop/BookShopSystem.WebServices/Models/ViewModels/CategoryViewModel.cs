using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryViewModel(Category category)
        {
            Name = category.Name;
            Id = category.Id;
        }
    }
}