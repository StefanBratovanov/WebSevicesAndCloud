using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookShopSystem.Models;

namespace BookShopSystem.WebServices.Models.BindingModels
{
    public class BookBindingModel
    {
        [Required(ErrorMessage = "Title is mandatory!")]
        [MinLength(5)]
        public string Title { get; set; }

        [Required]
        [MinLength(5)]
        public string Description { get; set; }

        [Required]
        public EditionType EditionType { get; set; }

        [Required(ErrorMessage = "Price is mandatory!")]
        public decimal Price { get; set; }

        [Required]
        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Age Restriction is mandatory!")]
        public Age AgeRestriction { get; set; }

        [Required(ErrorMessage = "Author Id is mandatory!")]
        public int AuthorId { get; set; }
    }
}