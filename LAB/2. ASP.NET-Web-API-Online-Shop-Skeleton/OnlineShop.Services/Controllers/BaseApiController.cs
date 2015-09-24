using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineShop.Data;

namespace OnlineShop.Services.Controllers
{
    public class BaseApiController : ApiController
    {

        public BaseApiController()
            : this(new OnlineShopContext())
        {

        }
        public BaseApiController(OnlineShopContext data)
        {
            this.Data = data;
        }

        protected OnlineShopContext Data { get; set; }
    }


}
