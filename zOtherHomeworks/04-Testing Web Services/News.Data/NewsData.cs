using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.Data.Repositories;
using News.Models;

namespace News.Data
{
    public class NewsData : INewsData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public NewsData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public INewsRepository<Models.News> News
        {
            get { return this.GetRepository<Models.News>(); }
        }
        public INewsRepository<ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }


        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public int SaveChangesAsync()
        {
            return this.context.SaveChanges();
        }

        private INewsRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof (T);
            if (!this.repositories.ContainsKey(type))
            {
                var typeOfRepository = typeof (GenericRepository<T>);
                var repository = Activator.CreateInstance(
                    typeOfRepository, this.context);

                this.repositories.Add(type, repository);
            }

            return (INewsRepository<T>) this.repositories[type];
        }

    }
}

