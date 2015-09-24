

using News.Data.Repositories;

namespace News.Data.DataClasses
{
    public interface INewsData
    {
        NewsRepository NewsRepository { get; }

        int SaveChanges();
    }
}
