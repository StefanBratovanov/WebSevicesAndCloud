using System.Linq;

namespace BookShopService.Models.ViewModels
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IQueryable Books { get; set; }
    }
}