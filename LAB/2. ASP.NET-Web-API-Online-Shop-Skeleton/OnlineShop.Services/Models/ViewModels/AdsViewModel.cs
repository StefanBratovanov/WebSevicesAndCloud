using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.Services.Models.ViewModels
{
    public class AdsViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime PostedOn { get; set; }

        public OwnerViewMOdel Owner { get; set; }

        public AdTypeViewModel Type { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        public static Expression<Func<Ad, AdsViewModel>> Create
        {
            get
            {
                return a => new AdsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    PostedOn = a.PostedOn,
                    Owner = new OwnerViewMOdel
                    {
                        Id = a.OwnerId,
                        UserName = a.Owner.UserName
                    },
                    Type = new AdTypeViewModel()
                    {
                        TypeName = a.Type.Name
                    },
                    Categories = a.Categories.Select(c => new CategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                };
            }
        }

    }
}






/*
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.Services.Models.ViewModels
{
    public class AdsViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime PostedOn { get; set; }

        public OwnerViewMOdel Owner { get; set; }

        public AdType Type { get; set; }

        public  ICollection<CategoryViewModel> Categories { get; set; }

        public AdsViewModel(Ad ad)
        {
            Id = ad.Id;
            Name = ad.Name;
            Description = ad.Description;
            Price = ad.Price;
            PostedOn = ad.PostedOn;
            Owner = new OwnerViewMOdel()
            {
                Id = ad.OwnerId,
                UserName = ad.Owner.UserName
            };
            Type = ad.Type;

            Categories = new List<CategoryViewModel>();

            foreach (var category in ad.Categories)
            {
                Categories.Add(new CategoryViewModel(category));
            }

        }

    }
}

*/