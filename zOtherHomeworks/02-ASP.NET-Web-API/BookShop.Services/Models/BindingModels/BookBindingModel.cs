using System;
using BookShop.Models;

namespace BookShopService.Models.BindingModels
{
    public class BookBindingModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public EditionType EditionType { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public decimal Price { get; set; }
        public int Copies { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int AuthorId { get; set; }
        public string Categories { get; set; }
    }
}