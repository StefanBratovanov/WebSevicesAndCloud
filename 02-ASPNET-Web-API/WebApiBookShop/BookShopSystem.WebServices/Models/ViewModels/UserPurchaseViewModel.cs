using System;
using System.Linq.Expressions;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.ViewModels
{
    public class UserPurchaseViewModel
    {
        public string Username { get; set; }

        public string BookTitle { get; set; }

        public decimal Price { get; set; }

        public DateTime? DateOfPurchase { get; set; }

        public bool IsRecalled { get; set; }

        public static Expression<Func<Purchase, UserPurchaseViewModel>> Create
        {
            get
            {
                return p => new UserPurchaseViewModel
                {
                    Username = p.ApplicationUser.UserName,
                    BookTitle = p.Book.Title,
                    Price = p.Price,
                    DateOfPurchase = p.DateOfPurchase,
                    IsRecalled = p.IsRecalled
                };
            }
        }

    }
}