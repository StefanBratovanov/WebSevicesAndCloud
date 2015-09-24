using System;
using System.Collections.Generic;
using BookShop.Models;

namespace BookShopService.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public EditionType EditionType { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public virtual IEnumerable<string> Categories { get; set; }  
    }
}