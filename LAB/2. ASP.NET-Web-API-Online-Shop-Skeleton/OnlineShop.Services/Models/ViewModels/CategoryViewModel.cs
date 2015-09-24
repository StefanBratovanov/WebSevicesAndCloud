using System;

using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Services.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //public static Expression<Func<Category, CategoryViewModel>> Create
        //{
        //    get
        //    {
        //        return c => new CategoryViewModel()
        //        {
        //            Id = c.Id,
        //            Name = c.Name
        //        };
        //    }
        //}
    }
}

/*
 public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public CategoryViewModel(Category cat)
        {
            Id = cat.Id;
            Name = cat.Name;
        }
    }
*/