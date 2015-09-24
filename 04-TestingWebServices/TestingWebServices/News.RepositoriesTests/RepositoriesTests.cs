using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using News.Data;
using News.Data.Repositories;
using News.Models;

namespace News.RepositoriesTests
{
    [TestClass]
    public class RepositoriesTests
    {
        private static TransactionScope tran;

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
        public void Add_NewsRecord_Should_Add_NewsRecord_to_DB()
        {
            //Arrange
            NewsRepository repo = new NewsRepository(new NewsContext());
            var news = new NewsRecord()
            {
                Title = "HardShit",
                Content = "Hard Shit in the mix",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };

            //Act
            repo.Add(news);
            repo.SaveChanges();

            //Assert
            var newsFromDb = repo.FindById(news.Id);

            Assert.IsNotNull(newsFromDb, "NewsRecord is not added to database");
            Assert.IsTrue(newsFromDb.Id != 0, "NewsRecord must have id !=0");
            Assert.AreEqual(news.Title, newsFromDb.Title, "Title is not corect");
            Assert.AreEqual(news.Content, newsFromDb.Content, "Content is not corect");
            Assert.AreEqual(news.PublishDate, newsFromDb.PublishDate, "Date is not corect");
        }

        [TestMethod]
        public void List_NewsRecords_Should_Return_Correct_List()
        {
            //Arrange
            NewsRepository repo = new NewsRepository(new NewsContext());
            var countBeforeAdding = repo.GetAll().Count();

            var news = new NewsRecord()
            {
                Title = "HardShit",
                Content = "Hard Shit in the mix",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };
            var news2 = new NewsRecord()
            {
                Title = "SmokeShit",
                Content = "Smoke Shit in the mix",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };

            var listNews = new List<NewsRecord>();
            listNews.Add(news);
            listNews.Add(news2);

            var newsBeforeAdd = repo.GetAll()
               .Select(n => n.Title)
               .ToList();

            var fakeNewsTitles = listNews
                .Select(n => n.Title)
                .ToList();

            fakeNewsTitles.AddRange(newsBeforeAdd);
            fakeNewsTitles.Sort();

            var fakeNewsTitlesOrd = fakeNewsTitles;
                 

            //Act
            repo.Add(news);
            repo.Add(news2);
            repo.SaveChanges();

            var allNews = repo.GetAll()
                .OrderBy(n => n.Title)
                .Select(n => n.Title)
                .ToList();

            //Assert
            Assert.AreEqual(2 + countBeforeAdding, repo.GetAll().Count());
            CollectionAssert.AreEqual(fakeNewsTitlesOrd, allNews);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Add_Null_News_Record_Should_Throw_Exception()
        {
            // Arrange -> prapare the objects
            NewsRepository repo = new NewsRepository(new NewsContext());
            var invalidNewsRecord = new NewsRecord()
            {
                Title = "null shit",
                Content = null
            };

            // Act -> perform some logic
            repo.Add(invalidNewsRecord);
            repo.SaveChanges();

            // Assert -> expect an exception
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Add_Empty_Sting_Title_News_Record_Should_Throw_Exception()
        {
            // Arrange -> prapare the objects
            NewsRepository repo = new NewsRepository(new NewsContext());
            var invalidNewsRecord = new NewsRecord()
            {
                Title = "",
                Content = "Oh big Shit"
            };

            // Act -> perform some logic
            repo.Add(invalidNewsRecord);
            repo.SaveChanges();

            // Assert -> expect an exception
        }

        [TestMethod]
        public void Modify_Existing_Data_Should_Update_The_Data()
        {
            NewsRepository repo = new NewsRepository(new NewsContext());
            var news = new NewsRecord()
            {
                Title = "UnModifiedShit",
                Content = "See me updated",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };

            repo.Add(news);
            repo.SaveChanges();

            var newsFromDB = repo.GetAll().FirstOrDefault(n => n.Title == "UnModifiedShit");
            newsFromDB.Title = "ModifiedShit";

            repo.Update(news);
            repo.SaveChanges();

            var changedNews = repo.GetAll().FirstOrDefault(n => n.Title == "ModifiedShit");

            Assert.AreEqual("ModifiedShit", changedNews.Title);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Modify_Existing_Data_With_Incorrect_Data_Should_Throw_Exception()
        {
            NewsRepository repo = new NewsRepository(new NewsContext());
            var news = new NewsRecord()
            {
                Title = "UnModifiedShit",
                Content = "See me updated",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };

            repo.Add(news);
            repo.SaveChanges();

            var newsFromDB = repo.GetAll().FirstOrDefault(n => n.Title == "UnModifiedShit");
            newsFromDB.Title = "";

            repo.Update(news);
            repo.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Modify_Non_Existing_Data_Should_Throw_Exception()
        {
            NewsRepository repo = new NewsRepository(new NewsContext());

            var newsFromDB = repo.GetAll().FirstOrDefault(n => n.Id == 1500);
            newsFromDB.Title = "Ihaaaaa";

            repo.Update(newsFromDB);
            repo.SaveChanges();
        }

        [TestMethod]
        public void Delete_Existing_Data_Should_Delete_The_Data()
        {
            NewsRepository repo = new NewsRepository(new NewsContext());
            var news = new NewsRecord()
            {
                Title = "UnModifiedShit",
                Content = "See me updated",
                PublishDate = new DateTime(2015, 08, 30, 14, 04, 30)
            };

            repo.Add(news);
            repo.SaveChanges();

            var countBeforeDeletion = repo.GetAll().Count();

            var newsFromDB = repo.GetAll().FirstOrDefault(n => n.Title == "UnModifiedShit");
            repo.Delete(newsFromDB);
            repo.SaveChanges();

            Assert.AreEqual(countBeforeDeletion - 1, repo.GetAll().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Delete_Non_Existing_Data_Should_Throw_Exception()
        {
            NewsRepository repo = new NewsRepository(new NewsContext());

            var newsFromDB = repo.GetAll().FirstOrDefault(n => n.Id == 1500);

            repo.Delete(newsFromDB);
            repo.SaveChanges();

        }

    }
}
