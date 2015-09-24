
using System.Linq;


namespace News.Data.Repositories
{
    public interface IRepository<T>
    {
        T FindById(int id);

        IQueryable<T> GetAll();

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void DeleteById(int id);

        int SaveChanges();
    }
}
