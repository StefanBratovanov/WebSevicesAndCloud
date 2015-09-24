using System;

namespace BookShopService.Models.BindingModels
{
    public class BookPurchaseBindingModel
    {
        public string User { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public bool IsRecalled { get; set; }
    }
}