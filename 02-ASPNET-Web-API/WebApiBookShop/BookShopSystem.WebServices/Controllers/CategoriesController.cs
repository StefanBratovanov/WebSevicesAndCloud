using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BookShopSystem.Data;
using BookShopSystem.Models;
using BookShopSystem.WebServices.Models.ViewModels;

namespace BookShopSystem.WebServices.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // GET: api/categories
        public IHttpActionResult GetCategories()
        {
            var categories = context.Categories;

            if (!categories.Any())
            {
                return this.NotFound();
            }

            var data = new List<CategoryViewModel>();

            foreach (var category in categories)
            {
                CategoryViewModel cat = new CategoryViewModel(category);
                data.Add(cat);
            }

            return this.Ok(data);
        }

        // GET: api/categories/{id}
        public IHttpActionResult GetCategoryById(int id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return this.NotFound();
            }

            CategoryViewModel categoryInfo = new CategoryViewModel(category);

            return this.Ok(categoryInfo);
        }

        // PUT /api/categories/{id}
        [HttpPut]
        public IHttpActionResult EditCategory(int id, [FromBody]CategoryBindingModel model)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Model is empty!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var categoryExists = context.Categories.FirstOrDefault(c => c.Name == model.Name);

            if (categoryExists != null)
            {
                return this.BadRequest("Category already exists!");
            }

            category.Name = model.Name;

            context.SaveChanges();

            var returnData = context.Categories
                .Where(c => c.Id == category.Id)
                .Select(c => new
                {
                    c.Name
                })
                .FirstOrDefault();

            return this.Ok(returnData);
        }

        //DELETE	/api/books/{id}

        public IHttpActionResult DeleteCategoryById(int id)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return this.NotFound();
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            return this.Ok();
        }

        // POST: api/categories
        [HttpPost]
        public IHttpActionResult PostCategory([FromBody]CategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var categoryExists = context.Categories.FirstOrDefault(c => c.Name == model.Name);

            if (categoryExists != null)
            {
                return this.BadRequest("Category already exists!");
            }

            var categoryToAdd = new Category()
            {
                Name = model.Name
            };

            context.Categories.Add(categoryToAdd);

            context.SaveChanges();

            var data = new CategoryViewModel(categoryToAdd);

            return this.Ok(data);
        }

    }
}