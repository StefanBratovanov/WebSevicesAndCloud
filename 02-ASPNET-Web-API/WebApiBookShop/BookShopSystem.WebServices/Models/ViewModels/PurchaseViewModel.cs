using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class PurchaseViewModel
    {
        public string User { get; set; }

        public decimal Price { get; set; }

        public string BookTitle { get; set; }

        public PurchaseViewModel(Purchase purchase)
        {
            User = purchase.ApplicationUser.UserName;
            BookTitle = purchase.Book.Title;
            Price = purchase.Book.Price;
        }
    }
}