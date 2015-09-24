using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using News.Data.Repositories;
using News.Models;
using News.Services.Controllers;
using News.Services.Models;
using Newtonsoft.Json;


namespace News.ControllersTesting
{
    [TestClass]
    public class ControllersTests
    {
        [TestMethod]
        public void List_All_Items_Should_Work_Correctly()
        {
            var mockRepo = new Mock<IRepository<NewsRecord>>();
            var news = new List<NewsRecord>()
            {
                new NewsRecord()
                {
                    Title = "HopHop1",
                    Content = "The hoppy hop",
                    PublishDate = DateTime.Now
                },
                new NewsRecord()
                {
                    Title = "HopHop2",
                    Content = "The hoppy hop2",
                    PublishDate = DateTime.Now.AddDays(+5)
                },
                new NewsRecord()
                {
                    Title = "HopHop3",
                    Content = "The hoppy hop3",
                    PublishDate = DateTime.Now.AddDays(+10)
                }

            };

            mockRepo.Setup(r => r.GetAll()).Returns(news.AsQueryable());

            var controller = new NewsController(mockRepo.Object);
            this.SetupController(controller, "NewsController");

            var response = controller.GetAll().ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = response.Content.ReadAsStringAsync().Result;
            var resultNews = JsonConvert.DeserializeObject<List<NewsRecord>>(content);

            for (int i = 0; i < resultNews.Count; i++)
            {
                Assert.AreEqual(news[i].Id, resultNews[i].Id);
                Assert.AreEqual(news[i].Title, resultNews[i].Title);
                Assert.AreEqual(news[i].Content, resultNews[i].Content);
                Assert.AreEqual(news[i].PublishDate, resultNews[i].PublishDate);
            }
        }

        [TestMethod]
        public void Add_Correct_NewsRecord_Should_Add_NewsRecord_to_DB()
        {
            var mockRepo = new Mock<IRepository<NewsRecord>>();
            var news = new List<NewsRecord>()
            {
                new NewsRecord()
                {
                    Title = "HopHop1",
                    Content = "The hoppy hop",
                    PublishDate = DateTime.Now
                },
                new NewsRecord()
                {
                    Title = "HopHop2",
                    Content = "The hoppy hop2",
                    PublishDate = DateTime.Now.AddDays(+5)
                },
                new NewsRecord()
                {
                    Title = "HopHop3",
                    Content = "The hoppy hop3",
                    PublishDate = DateTime.Now.AddDays(+10)
                }
            };

            mockRepo.Setup(r => r.Add(It.IsNotNull<NewsRecord>()))
                .Callback((NewsRecord n) => news.Add(n));

            var controller = new NewsController(mockRepo.Object);
            this.SetupController(controller, "NewsController");

            var newsToPost = new NewsRecordBindingModel()
            {
                Title = "Lets see",
                Content = "Messy shit",
            };

            var response = controller.PostNews(newsToPost).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(news.Count == 4);
            Assert.AreEqual("Messy shit", news.Last().Content);
            Assert.AreEqual(newsToPost.Title, news.Last().Title);
            Assert.IsNotNull(news.Last().PublishDate);
        }

        [TestMethod]
        public void Add_InCorrect_NewsRecord_Should_Throw_Exception()
        {
            var mockRepo = new Mock<IRepository<NewsRecord>>();
            var news = new List<NewsRecord>()
            {
                new NewsRecord()
                {
                    Title = "HopHop1",
                    Content = "The hoppy hop",
                    PublishDate = DateTime.Now
                },
                new NewsRecord()
                {
                    Title = "HopHop2",
                    Content = "The hoppy hop2",
                    PublishDate = DateTime.Now.AddDays(+5)
                },
                new NewsRecord()
                {
                    Title = "HopHop3",
                    Content = "The hoppy hop3",
                    PublishDate = DateTime.Now.AddDays(+10)
                }
            };

            mockRepo.Setup(r => r.Add(It.IsNotNull<NewsRecord>()))
                .Callback((NewsRecord n) => news.Add(n));

            var controller = new NewsController(mockRepo.Object);
            this.SetupController(controller, "NewsController");
            controller.ModelState.AddModelError("key", "error message");

            var newsToPost = new NewsRecordBindingModel()
            {
                Title = "",
                Content = "Messy shit",
            };

            var response = controller.PostNews(newsToPost).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Modify_Existing_Data_Should_Update_The_Data()
        {
            var mockRepo = new Mock<IRepository<NewsRecord>>();
            var news = new List<NewsRecord>()
            {
                new NewsRecord
                {
                    Id = 1,
                    Title = "HopHop1",
                    Content = "The hoppy hop",
                    PublishDate = DateTime.Now
                },
                new NewsRecord
                {
                    Id = 2,
                    Title = "HopHop2",
                    Content = "The hoppy hop2",
                    PublishDate = DateTime.Now.AddDays(+5)
                },
                new NewsRecord
                {
                    Id = 3,
                    Title = "HopHop3",
                    Content = "The hoppy hop3",
                    PublishDate = DateTime.Now.AddDays(+10)
                }
            };

            mockRepo.Setup(r => r.Update(It.IsAny<NewsRecord>()))
              .Callback((NewsRecord n) => news[news.FindIndex(x => x.Id == n.Id)] = n);

            var controller = new NewsController(mockRepo.Object);
            this.SetupController(controller, "NewsController");

            var newsFromRepo = news.SingleOrDefault(n => n.Title == "HopHop1");
            var newsFromRepoId = newsFromRepo.Id;

            var modifyModel = new NewsRecordBindingModel()
            {
                Title = "Changed Shit",
                Content = "Messy shit",
            };

            var response = controller.PutNewsRecord(newsFromRepoId, modifyModel).ExecuteAsync(CancellationToken.None).Result;

            var newsAfterUpdate = news.SingleOrDefault(n => n.Id == newsFromRepo.Id);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Changed Shit", newsAfterUpdate.Title);
        }

        [TestMethod]
        public void Delete_Existing_Data_Should_Delete_The_Data()
        {
            var mockRepo = new Mock<IRepository<NewsRecord>>();
            var news = new List<NewsRecord>()
            {
                new NewsRecord
                {
                    Id = 1,
                    Title = "HopHop1",
                    Content = "The hoppy hop",
                    PublishDate = DateTime.Now
                },
                new NewsRecord
                {
                    Id = 2,
                    Title = "HopHop2",
                    Content = "The hoppy hop2",
                    PublishDate = DateTime.Now.AddDays(+5)
                },
                new NewsRecord
                {
                    Id = 3,
                    Title = "HopHop3",
                    Content = "The hoppy hop3",
                    PublishDate = DateTime.Now.AddDays(+10)
                }
            };

            int id = 1;

            mockRepo.Setup(r => r.FindById(id)).Returns(news.AsQueryable().SingleOrDefault(item => item.Id == id));
            mockRepo.Setup(repo => repo.Delete(It.Is((NewsRecord n) => n.Id == id)))
                .Callback((NewsRecord n) => news.RemoveAt(news.FindIndex(x => x.Id == n.Id)));

            var controller = new NewsController(mockRepo.Object);
            this.SetupController(controller, "NewsController");

            var result = controller.DeleteNewsRecord(id).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(2, news.Count);
        }


        private void SetupController(ApiController controller, string controllerName)
        {
            string serverUrl = "http://sample-url.com";

            // Setup the Request object of the controller
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(serverUrl)
            };
            controller.Request = request;

            // Setup the configuration of the controller
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            controller.Configuration = config;

            // Apply the routes to the controller
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary
                {
                    { "controller", controllerName }
                });
        }
    }
}
