using News.Data.Repositories;
using News.Models;

namespace News.Data
{
    public interface INewsData
    {
        INewsRepository<Models.News> News { get; }

        INewsRepository<ApplicationUser> Users { get; }

        int SaveChanges();

    }
}
