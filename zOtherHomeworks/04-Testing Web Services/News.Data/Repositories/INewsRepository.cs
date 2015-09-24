using System.Linq;

namespace News.Data.Repositories
{
    public interface INewsRepository<T>
    {
        IQueryable<T> All();

        T Find(object id);

        void Add(T entity);

        void Delete(T entity);

        void Update(T entity);
    }
}
