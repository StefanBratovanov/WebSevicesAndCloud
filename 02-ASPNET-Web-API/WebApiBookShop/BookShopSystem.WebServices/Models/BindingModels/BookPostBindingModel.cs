using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShopSystem.WebServices.Models.ViewModels;

namespace BookShopSystem.WebServices.Models.BindingModels
{
    public class BookPostBindingModel : BookBindingModel
    {
        public string Categories { get; set; }          
    }
}