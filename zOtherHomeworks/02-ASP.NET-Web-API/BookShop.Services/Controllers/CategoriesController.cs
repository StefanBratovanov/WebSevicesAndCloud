using System.Linq;
using System.Web.Http;
using BookShop.Data;
using BookShop.Models;
using BookShopService.Models.BindingModels;
using BookShopService.Models.ViewModels;

namespace BookShopService.Controllers
{
    public class CategoriesController : ApiController
    {
        private readonly BookShopContext _context = new BookShopContext();

        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            var categories = _context.Categories.Select(c => new CategoriesModelView
            {
                Id = c.Id,
                Name = c.Name
            });

            return Ok(categories);
        }

        [HttpGet]
        public IHttpActionResult GetCategoryById(int id)
        {
            var category = _context.Categories
                .Select(c => new CategoriesModelView
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefault(c => c.Id == id);

            return Ok(category);
        }

        [HttpPut]
        public IHttpActionResult EditCategoryName(int id, [FromBody] CategoriesBindingModel categ)
        {
            if (!_context.Categories.Any(c => c.Name == categ.Name))
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == id);

                category.Name = categ.Name;

                _context.SaveChanges();

                return Ok("Name changed!");
            }

            return BadRequest("Category with that name already exists!");
        }

        [HttpDelete]
        public IHttpActionResult DeleteCategory(int id)
        {
            if (_context.Categories.Any(c => c.Id == id))
            {
                _context.Categories.Remove(_context.Categories.FirstOrDefault(c => c.Id == id));

                _context.SaveChanges();

                return Ok("Successfully deleted!");
            }

            return BadRequest("No such category exists!");
        }

        [HttpPost]
        public IHttpActionResult AddCategory([FromBody] CategoriesBindingModel categ)
        {
            if (_context.Categories.Any(c => c.Name == categ.Name))
            {
                return BadRequest("Category with that name already exists!");
            }

            _context.Categories.Add(new Category
            {
                Name = categ.Name
            });

            _context.SaveChanges();

            return Ok("Category successfully added!");
        }
    }
}