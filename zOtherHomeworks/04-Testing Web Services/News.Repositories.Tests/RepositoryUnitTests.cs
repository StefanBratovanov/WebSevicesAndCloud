using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using EntityFramework.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using News.Data;

namespace News.Repositories.Tests
{
    [TestClass]
    public class NewsRepositoryTests
    {
        private static TransactionScope tran;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            var dbContext = new NewsContext();
            dbContext.Database.CreateIfNotExists();
        }

        [TestInitialize]
        public void TestInit()
        {
            // Start a new temporary transaction
            tran = new TransactionScope();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            // Rollback the temporary transaction
            tran.Dispose();
        }

        [TestMethod]
        public void GetAllNews_ShouldReturnAllNews()
        {
            var context = new NewsContext();
            context.News.Delete();
            var firstNews = new Models.News()
            {
                Title = "Murder",
                Content = "A man was slaughterd in Sofia last night.",
                PublishDate = DateTime.Now
            };
            context.News.Add(firstNews);
            var secNews = new Models.News()
            {
                Title = "Eros Ramazzotti",
                Content = "Ramazzotti will sing in Sofia in September",
                PublishDate = DateTime.Now
            };
            context.News.Add(secNews);
            context.SaveChanges();
            var repo = new NewsRepository(context);
            var newsFromDB = repo.All().ToArray();
            Assert.IsNotNull(newsFromDB[0]);
            Assert.AreEqual(firstNews.Title, newsFromDB[0].Title);
            Assert.AreEqual(firstNews.Content, newsFromDB[0].Content);
            Assert.AreEqual(firstNews.PublishDate, newsFromDB[0].PublishDate);
            Assert.IsTrue(newsFromDB[0].Id != 0);

            Assert.IsNotNull(newsFromDB[1]);
            Assert.AreEqual(secNews.Title, newsFromDB[1].Title);
            Assert.AreEqual(secNews.Content, newsFromDB[1].Content);
            Assert.AreEqual(secNews.PublishDate, newsFromDB[1].PublishDate);
            Assert.IsTrue(newsFromDB[1].Id != 0);
        }

        [TestMethod]
        public void CreateNews_WhenNewsIsAddedToDbWithCorrectData_ShouldReturnNews()
        {
            // Arrange -> prapare the objects

            var repo = new NewsRepository(new NewsContext());
            var news = new Models.News()
            {
                Title = "Sample New News",
                PublishDate = DateTime.Now,
                Content = "Sample New News Content"
            };

            // Act -> perform some logic
            repo.Add(news);
            repo.SaveChanges();

            // Assert -> validate the results
            var newsFromDb = repo.Find(news.Id);

            Assert.IsNotNull(newsFromDb);
            Assert.AreEqual(news.Title, newsFromDb.Title);
            Assert.AreEqual(news.Content, newsFromDb.Content);
            Assert.AreEqual(news.PublishDate, newsFromDb.PublishDate);
            Assert.IsTrue(newsFromDb.Id != 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void CreateNews_WhenNewsIsInvalid_ShouldThrowException()
        {
            var repo = new NewsRepository(new NewsContext());
            var invalidNews = new Models.News()
            {
                Title = null,
                Content = "bla bla",
                PublishDate = DateTime.Now
            };

            repo.Add(invalidNews);
            repo.SaveChanges();

            // Assert -> expect an exception
        }

        [TestMethod]
        public void ModifyNews_WhenExistingNewsIsModifiedWithCorrectData_ShouldReturnNews()
        {
            var context = new NewsContext();
            context.News.Delete();
            var news = new Models.News()
            {
                Title = "Murder",
                Content = "A man was slaughterd in Sofia last night.",
                PublishDate = DateTime.Now
            };
            context.News.Add(news);
            context.SaveChanges();
            var repo = new NewsRepository(context);
            var newsToChange = repo.Find(news.Id);
            newsToChange.Title = "Murder committed last night";
            repo.SaveChanges();
            var newsFromDb = repo.Find(news.Id);
            Assert.IsNotNull(newsFromDb);
            Assert.AreEqual(newsToChange.Title, newsFromDb.Title);
            Assert.AreEqual(news.Content, newsFromDb.Content);
            Assert.AreEqual(news.PublishDate, newsFromDb.PublishDate);
            Assert.IsTrue(newsFromDb.Id != 0);
        }
        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void ModifyNews_WhenExistingNewsIsModifiedWithIncorrectData_ShouldThrowException()
        {
            var context = new NewsContext();
            context.News.Delete();
            var news = new Models.News()
            {
                Title = "Murder",
                Content = "A man was slaughterd in Sofia last night.",
                PublishDate = DateTime.Now
            };
            context.News.Add(news);
            context.SaveChanges();
            var repo = new NewsRepository(context);
            var newsToChange = repo.Find(news.Id);
            newsToChange.Title = null;
            repo.SaveChanges();
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ModifyNews_WhenModifingNonExistingNews_ShouldThrowException()
        {
            var context = new NewsContext();
            context.News.Delete();
            context.SaveChanges();
            var repo = new NewsRepository(context);
            var newsToChange = repo.Find(1);
            newsToChange.Title = "New title";
            repo.SaveChanges();
        }
        [TestMethod]
        public void DeleteNews_WhenDeletingExistingNews_ShouldDeleteNews()
        {
            var context = new NewsContext();
            context.News.Delete();
            var news = new Models.News()
            {
                Title = "Murder",
                Content = "A man was slaughterd in Sofia last night.",
                PublishDate = DateTime.Now
            };
            context.News.Add(news);
            context.SaveChanges();
            var repo = new NewsRepository(context);
            repo.Delete(news);
            repo.SaveChanges();
            var newsFromDb = repo.All().ToArray();
            Assert.AreEqual(0, newsFromDb.Count());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteNews_WhenDeletingNonExistingNews_ShouldThrowException()
        {
            var context = new NewsContext();
            context.News.Delete();
            context.SaveChanges();
            var repo = new NewsRepository(context);
            var newsToDelete = repo.Find(1);
            repo.Delete(newsToDelete);
            repo.SaveChanges();
        }
    }
}
