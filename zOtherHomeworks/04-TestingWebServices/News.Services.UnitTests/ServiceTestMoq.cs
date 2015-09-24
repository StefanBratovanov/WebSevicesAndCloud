namespace News.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using News.Models;
    using News.Repositories;
    using News.Services.Controllers;
    using News.Services.Models;

    [TestClass]
    public class ServiceTestMoq
    {
        private MockContainer mock;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareNews();
        }

        [TestMethod]
        public void ListAllNewsShouldReturnOk()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);

            var response = controller.GetAllNews()
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(
                this.mock.NewsRepositoryMock.Object.All().Count(),
                3,
                "Counts of news are not currect.");
            Assert.AreEqual(
                response.StatusCode, 
                HttpStatusCode.OK, 
                "Expect status code OK.");
        }

        [TestMethod]
        public void CreateNewsAddToRepositoryShouldReturnCreated()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            var newsBind = new NewsBindingModel
                               {
                                   Title = "Some new news", 
                                   Content = "Content", 
                                   PublishedDate = DateTime.Now
                               };

            var response = controller.CreateNews(newsBind).ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public void CreateNewsWithWrongBindingModelShouldReturnBadRequest()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            var newsBind = new NewsBindingModel();

            controller.ModelState.AddModelError("BadRequest", "Content can't be empty.");
            var response = controller.CreateNews(newsBind).ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ModifyExistingNewsShouldReturnOk()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            var newsBind = new NewsBindingModel { Title = "Edit", Content = "Edit", PublishedDate = DateTime.Now };

            var response = controller.EditNews(1, newsBind).ExecuteAsync(CancellationToken.None).Result;

            var editedNews = this.mock.NewsRepositoryMock.Object.Find(1);

            Assert.AreEqual(editedNews.Title, "Edit");
            Assert.AreEqual(editedNews.Content, "Edit");
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void ModifyExistingNewsIncorrectDataShouldReturnBadReques()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            controller.ModelState.AddModelError("BadRequest", "Empty binding model.");

            var response = controller.EditNews(1, new NewsBindingModel())
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ModifyNonExistingNewsShouldReturnBadRequest()
        {
            var newsRepository = new Mock<IRepository<News>>();
            var controller = new NewsController(newsRepository.Object);
            this.SetupController(controller);

            var response = controller.EditNews(3, new NewsBindingModel())
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeleteNewsShouldReturnOk()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            
            var response = controller.DeleteNews(1).ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteNonExistingNewsShouldReturnNotFound()
        {
            var controller = new NewsController(this.mock.NewsRepositoryMock.Object);
            this.SetupController(controller);
            
            var response = controller.DeleteNews(12).ExecuteAsync(CancellationToken.None).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        private void SetupController(NewsController controller)
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
        }
    }
}