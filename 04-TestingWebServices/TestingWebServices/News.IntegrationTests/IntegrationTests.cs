
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using News.Data;
using News.Models;
using News.Services;
using Owin;


namespace News.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        private TestServer httpTestServer;
        private HttpClient httpClient;

        [TestInitialize]
        public void TestInit()
        {
            // Start OWIN testing HTTP server with Web API support
            this.httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                appBuilder.UseWebApi(config);
            });
            this.httpClient = httpTestServer.HttpClient;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.httpTestServer.Dispose();
        }

        [TestMethod]
        public void List_All_Emty_Items_Should_Work_Correctly_200_Ok()
        {
            // Arrange
            this.CleanDatabase();
            // Act
            var httpResponse = httpClient.GetAsync("/api/news").Result;
            var news = httpResponse.Content.ReadAsAsync<List<NewsRecord>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual(0, news.Count);
        }

        [TestMethod]
        public void List_All_Items_Should_Work_Correctly_200_OK()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();

            database.News.Add(new NewsRecord()
            {
                Title = "HopHop1",
                Content = "The hoppy hop",
                PublishDate = DateTime.Now
            });

            database.News.Add(new NewsRecord
            {
                Title = "HopHop2",
                Content = "The hoppy hop2",
                PublishDate = DateTime.Now.AddDays(+5)
            });

            database.SaveChanges();

            // Act
            var httpResponse = httpClient.GetAsync("/api/news").Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<List<NewsRecord>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual(2, newsFromService.Count);
        }

        [TestMethod]
        public void Add_Correct_NewsRecord_Should_Add_NewsRecord_to_DB_201_Created()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            // Act
            int id = 1;
            var postContent = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("Title", "112233"),
                new KeyValuePair<string, string>("Content", "223344"),
            });
            var httpResponse = httpClient.PostAsync("/api/news", postContent).Result;
            
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;
           // var httpResponse1 = httpClient.PostAsJsonAsync("/api/news", postContent).Result.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual("112233", newsFromService.Title);
            Assert.AreEqual("223344", newsFromService.Content);
            Assert.AreEqual(1, database.News.Count());

        }

        [TestMethod]
        public void Add_InCorrect_NewsRecord_Should_Return_400_BadRequest()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            var postContent = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("Title", "112233")
            });

            // Act
            var httpResponse = httpClient.PostAsync("/api/news", postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual(0, database.News.Count());
        }

        [TestMethod]
        public void Modify_Existing_Data_Should_Work_Correctly_200_Ok()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            database.News.Add(new NewsRecord()
            {
                Title = "HopHop1",
                Content = "The hoppy hop",
                PublishDate = DateTime.Now
            });

            database.News.Add(new NewsRecord
            {
                Title = "HopHop2",
                Content = "The hoppy hop2",
                PublishDate = DateTime.Now.AddDays(+5)
            });

            database.SaveChanges();

            var id = database.News.First().Id;
            var postContent = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("Title", "Title"),
                new KeyValuePair<string, string>("Content", "Content"), 
                new KeyValuePair<string, string>("Id", id.ToString()), 
            });


            var endpointUrl = string.Format("/api/news?id={0}", id);
            // Act
            var httpResponse = httpClient.PutAsync(endpointUrl, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }


        [TestMethod]
        public void Modify_Existing_With_Incorrect_Data_Return_400_BadRequest()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            database.News.Add(new NewsRecord()
            {
                Title = "HopHop1",
                Content = "The hoppy hop",
                PublishDate = DateTime.Now
            });

            database.News.Add(new NewsRecord
            {
                Title = "HopHop2",
                Content = "The hoppy hop2",
                PublishDate = DateTime.Now.AddDays(+5)
            });

            database.SaveChanges();

            var id = database.News.First().Id;
            var postContent = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("Title", "ssssaaaa"),
                new KeyValuePair<string, string>("Content", "ssasasas"),

            //    new KeyValuePair<string, string>("Content", "Content"), 
            });


            var endpointUrl = string.Format("/api/news?id={0}", id);
            // Act

            var httpResponse = httpClient.PutAsync(endpointUrl, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_Existing_Data_Should_Return200_Ok()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            database.News.Add(new NewsRecord()
            {
                Title = "HopHop1",
                Content = "The hoppy hop",
                PublishDate = DateTime.Now
            });

            database.News.Add(new NewsRecord
            {
                Title = "HopHop2",
                Content = "The hoppy hop2",
                PublishDate = DateTime.Now.AddDays(+5)
            });

            database.SaveChanges();

            var id = database.News.First().Id;

            var endpointUrl = string.Format("/api/news/{0}", id);
            // Act
            var httpResponse = httpClient.DeleteAsync(endpointUrl).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(1, database.News.Count());
        }


        [TestMethod]
        public void Delete_Existing_With_Incorrect_Data_Return_400_BadRequest()
        {
            // Arrange
            CleanDatabase();
            var database = new NewsContext();
            database.News.Add(new NewsRecord()
            {
                Title = "HopHop1",
                Content = "The hoppy hop",
                PublishDate = DateTime.Now
            });

            database.News.Add(new NewsRecord
            {
                Title = "HopHop2",
                Content = "The hoppy hop2",
                PublishDate = DateTime.Now.AddDays(+5)
            });

            database.SaveChanges();

            var id = 0;

            var endpointUrl = string.Format("/api/news/{0}", id);
            // Act
            var httpResponse = httpClient.DeleteAsync(endpointUrl).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<NewsRecord>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        private void CleanDatabase()
        {
            // Clean all data in all database tables
            var dbContext = new NewsContext();

          

            foreach (var newsR in dbContext.News)
            {
                dbContext.News.Remove(newsR);
            }

            dbContext.SaveChanges();
        }
    }
}
