using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using News.Data;
using News.Services.Controllers;
using News.Services_new.Infrastructure;
using News.Services_new.Models;

namespace _03.Unit_Tests_API_Controllers
{
    [TestClass]
    public class ControllersUnitTests
    {
        private MockContainer mocks;
        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }
        [TestMethod]
        public void GetAllNews_Should_Return_All_News()
        {
            var fakeNews = this.mocks.NewsRepositoryMock.Object.All();
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(r => r.News.All()).Returns(fakeNews.AsQueryable());
            var mockUserIdProvider = new Mock<IUserIdProvider>();
            var newsController = new NewsController(mockContext.Object, mockUserIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();
            var response = newsController.GetAllNews().ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var newsResponse = response.Content.ReadAsAsync<List<NewsViewModel>>().Result;

            var orderedFakeNews = fakeNews
                .OrderByDescending(n => n.PublishDate)
                .Select(NewsViewModel.Create)
                .ToList();

            CollectionAssert.AreEqual(orderedFakeNews, newsResponse, new NewsComparer());
        }
        [TestMethod]
        public void CreateNews_With_Correct_Data_Should_Successfully_Add_To_Repository()
        {
            var newsList = new List<News.Models.News>();
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }
            this.mocks.NewsRepositoryMock
                .Setup(r => r.Add(It.IsAny<News.Models.News>()))
                .Callback((News.Models.News news) =>
                {
                    news.Author = fakeUser;
                    news.PublishDate = DateTime.Now;
                    newsList.Add(news);
                });
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var newNews = new CreateNewsBindingModel()
            {
                Title = "new news",
                Content = "Nothing to say"
            };
            var response = newsController.CreateNews(newNews).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(1, newsList.Count);
            Assert.AreEqual(newNews.Title, newsList[0].Title);
        }
        [TestMethod]
        public void CreateNews_With_Incorrect_Data_Should_Return_400BadRequest()
        {
            var newsList = new List<News.Models.News>();
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }
            this.mocks.NewsRepositoryMock
                .Setup(r => r.Add(It.IsAny<News.Models.News>()))
                .Callback((News.Models.News news) =>
                {
                    news.Author = fakeUser;
                    news.PublishDate = DateTime.Now;
                    newsList.Add(news);
                });
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var newNews = new CreateNewsBindingModel()
            {
                Content = "news"
            };
            var response = newsController.CreateNews(newNews).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
            Assert.AreEqual(0, newsList.Count);
        }
        [TestMethod]
        public void ModifyExistingNews_With_Correct_Data_Should_Successfully_Modify_News_In_Repository()
        {
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var fakeNews = this.mocks.NewsRepositoryMock.Object.All().FirstOrDefault();
            var fakeNewsId = fakeNews.Id;
            var newNews = new CreateNewsBindingModel()
            {
                Title = "new news",
                Content = "Nothing to say"
            };
            var response = newsController.PutNews(newNews, fakeNewsId).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            var modifiedNews = this.mocks.NewsRepositoryMock.Object.All().FirstOrDefault(n => n.Id == fakeNewsId);
            var newsResponse = response.Content.ReadAsAsync<News.Models.News>().Result;
            Assert.AreEqual(newNews.Title, modifiedNews.Title);
            Assert.AreEqual(newNews.Title, newsResponse.Title);
        }
        [TestMethod]
        public void ModifyExistingNews_With_Incorrect_Data_Should_return_400BadRequest()
        {
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var fakeNews = this.mocks.NewsRepositoryMock.Object.All().FirstOrDefault();
            var fakeNewsId = fakeNews.Id;
            var newNews = new CreateNewsBindingModel()
            {
                Title = "new news",
            };
            var response = newsController.PutNews(newNews, fakeNewsId).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }
        [TestMethod]
        public void ModifyNonExistingNews_Should_return_400BadRequest()
        {
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var newNews = new CreateNewsBindingModel()
            {
                Title = "new news",
            };
            var response = newsController.PutNews(newNews, 1000).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }
        [TestMethod]
        public void Delete_Existing_News_Item_Should_Successfully_Delete_News_In_Repository()
        {
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }
            
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var newsToDelete = this.mocks.NewsRepositoryMock.Object.All().FirstOrDefault();

            var response = newsController.DeleteNews(newsToDelete.Id).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            var modifiedNews = this.mocks.NewsRepositoryMock.Object.All().FirstOrDefault(n => n.Title == newsToDelete.Title);
            Assert.AreEqual(null, modifiedNews);
        }
        [TestMethod]
        public void Delete_Non_Existing_News_Item_Should_Return_400BadRequest()
        {
            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News).Returns(this.mocks.NewsRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId()).Returns(fakeUser.Id);

            var newsController = new NewsController(mockContext.Object, mockIdProvider.Object);
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();

            var response = newsController.DeleteNews(1000).ExecuteAsync(CancellationToken.None).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        private class NewsComparer : Comparer<NewsViewModel>
        {
            public override int Compare(NewsViewModel x, NewsViewModel y)
            {
                return x.Title.CompareTo(y.Title);
            }
        }
    }
}
