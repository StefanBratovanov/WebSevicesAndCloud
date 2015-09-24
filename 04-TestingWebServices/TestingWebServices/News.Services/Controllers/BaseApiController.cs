using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using News.Data;
using News.Data.DataClasses;
using News.Data.Repositories;

namespace News.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        private NewsRepository repository;

        public BaseApiController()
        {
            var context = new NewsContext();
            this.repository = new NewsData(context).NewsRepository;
        }

        public NewsRepository NewsRepository
        {
            get { return this.repository; }
        }
    }
}
