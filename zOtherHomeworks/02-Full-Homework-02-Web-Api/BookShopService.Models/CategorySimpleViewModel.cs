using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class CategorySimpleViewModel
    {
        public CategorySimpleViewModel(Category category)
        {
            this.Name = category.Name;
        }

        public string Name { get; set; }
    }
}
