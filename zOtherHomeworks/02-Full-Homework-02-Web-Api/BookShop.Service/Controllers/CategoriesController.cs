using BookShop.Data;
using BookShop.Models;
using BookShopService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;

namespace BookShop.Service.Controllers
{
    public class CategoriesController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // Example: GET api/categories
        [HttpGet]
        [ResponseType(typeof(CategoryDetailedViewModel))]
        [EnableQuery]
        public IHttpActionResult GetAllCategories()
        {
            var categories = context.Categories;
            
            if (categories.Count() == 0)
	        {
		        return this.BadRequest(this.ModelState);
	        }

            var categorisView = new List<CategoryDetailedViewModel>();

            foreach (var category in categories)
            {
                categorisView.Add(new CategoryDetailedViewModel(category));
            }

            return this.Ok(categorisView);
        }

        // Example: GET api/categories/1
        [HttpGet]
        [ResponseType(typeof(CategoryDetailedViewModel))]
        public IHttpActionResult GetCategory(int id)
        {
            var category = context.Categories
                .Where(c => c.Id == id)
                .FirstOrDefault();

            if (category == null)
            {
                return this.BadRequest(this.ModelState);
            }

            return this.Ok(new CategoryDetailedViewModel(category));
        }

        // Example: PUT api/categoties/{id}
        // in request body: {"name"}
        [HttpPut]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult EditCategory(int id, [FromBody]CategoryBindingModel model)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            
            if (category == null)
            {
                return this.BadRequest("No such book.");
            }

            if (model == null)
            {
                return this.BadRequest(this.ModelState);
            }

            if (context.Categories.Select(c => c.Name).ToList().Contains(model.Name))
	        {
		        return this.BadRequest("Category with name {" + model.Name + "} already exist.");
	        }
            category.Name = model.Name;

            context.SaveChanges();

            return this.Ok("Category name has been edited.");    
        }

        // Example: DELETE api/categories/{id}
        [HttpDelete]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult DeleteCategory(int id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return this.BadRequest("No such category.");
            }
            context.Categories.Remove(category);
            context.SaveChanges();

            return this.Ok("Category has been deleted.");
        }

        // Example: POST api/categories
        // in request body: "Name":"{NewCategoryName}"
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult AddNewCategory([FromBody]CategoryBindingModel bindingCategory)
        {
            var categoryNames = context.Categories
                .Select(c => c.Name).ToList();
            if (categoryNames.Contains(bindingCategory.Name))
            {
                return this.BadRequest("Category already exist.");
            }

            var newCategory = new Category() { Name = bindingCategory.Name };
            context.Categories.Add(newCategory);
            context.SaveChanges();
            return this.Ok("Category {" + newCategory.Name + "} has been created.");
        }
    }
}
