using System;
using System.Data.Entity;
using System.Linq;
using News.Models;

namespace News.Data.Repositories
{
    public class NewsRepository : IRepository<NewsRecord>
    {
        private readonly DbContext dbContext;

        public NewsRepository(DbContext context)
        {
            this.dbContext = context;
        }

        public NewsRecord FindById(int id)
        {
            return this.dbContext.Set<NewsRecord>().Find(id);
        }

        public IQueryable<NewsRecord> GetAll()
        {
            return this.dbContext.Set<NewsRecord>();
        }

        public NewsRecord Add(NewsRecord entity)
        {
            this.dbContext.Set<NewsRecord>().Add(entity);
            return entity;
        }

        public void Update(NewsRecord entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void Delete(NewsRecord entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void DeleteById(int id)
        {
            var entity = this.FindById(id);
            this.ChangeState(entity, EntityState.Deleted);
        }

        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }

        private void ChangeState(NewsRecord NewsRecord, EntityState state)
        {
            var entry = this.dbContext.Entry(NewsRecord);
            if (entry.State == EntityState.Detached)
            {
                this.dbContext.Set<NewsRecord>().Attach(NewsRecord);
            }

            entry.State = state;
        }
    }
}
