namespace NewsRepository.Tests
{
    using System;
    using System.Data.Entity.Validation;
    using System.Transactions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using News.Data;
    using News.Models;
    using News.Repositories;

    [TestClass]
    public class NewsTest
    {
        private static TransactionScope scope;

        [TestInitialize]
        public void StartUp()
        {
            scope = new TransactionScope();
        }

        [TestCleanup]
        public void CleanUp()
        {
            scope.Dispose();
        }

        [TestMethod]
        public void GetAllNewsReturnNewsShouldReturnCorrectly()
        {
            var repo = new NewsRepository(new NewsContext());

            var news = new News
            {
                Title = "Test Title",
                Content = "Some test content.",
                PublishedDate = DateTime.Now
            };
            repo.Add(news);
            repo.SaveChanges();

            var newsDb = repo.Find(news.Id);

            Assert.IsNotNull(newsDb);
            Assert.AreEqual(news.Title, newsDb.Title);
            Assert.AreEqual(news.Content, newsDb.Content);
            Assert.AreEqual(news.PublishedDate, newsDb.PublishedDate);
        }
        
        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void CreateAndAddInvalidNewsToDbShouldReturnError()
        {
            var repo = new NewsRepository(new NewsContext());

            var news = new News { Title = null };
            repo.Add(news);
            repo.SaveChanges();
        }

        [TestMethod]
        public void ModifyExistingNewsWithCorrectData()
        {
            var repo = new NewsRepository(new NewsContext());

            var newsFromDb = repo.Find(1);

            newsFromDb.Title = "Edited Title";
            newsFromDb.Content = "Some new content";

            repo.SaveChanges();

            Assert.IsNotNull(newsFromDb);
            Assert.AreEqual(newsFromDb.Title, "Edited Title");
            Assert.AreEqual(newsFromDb.Content, "Some new content");
            Assert.AreEqual(1, newsFromDb.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void ModifyExistingNewsWithIncorrectData()
        {
            var repo = new NewsRepository(new NewsContext());

            var newsFromDb = repo.Find(1);

            newsFromDb.Title = null;

            repo.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ModifyNonExistingNewsShouldTrhowException()
        {
            var repo = new NewsRepository(new NewsContext());

            var newsFromDb = repo.Find(5);
            newsFromDb.Title = "NonExisting entity";
        }

        [TestMethod]
        public void DeleteExistingNews()
        {
            var repo = new NewsRepository(new NewsContext());

            var newsFromDb = repo.Find(1);

            repo.Delete(newsFromDb);
            repo.SaveChanges();

            Assert.IsNull(repo.Find(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryToDeleteNonExistingNews()
        {
            var repo = new NewsRepository(new NewsContext());

            var newsFromDb = repo.Find(5);

            repo.Delete(newsFromDb);
        }
    }
}