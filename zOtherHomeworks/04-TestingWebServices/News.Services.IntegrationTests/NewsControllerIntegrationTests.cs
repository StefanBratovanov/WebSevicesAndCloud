namespace News.Services.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using News.Data;
    using News.Models;

    using Owin;

    [TestClass]
    public class NewsControllerIntegrationTests
    {
        private const string NewsRequestUri = "api/news";

        private TestServer httpTestServer;

        private HttpClient httpClient;
        
        [TestInitialize]
        public void TestInit()
        {
            this.httpTestServer = TestServer.Create(
                appBuilder =>
                    {
                        var config = new HttpConfiguration();
                        WebApiConfig.Register(config);
                        appBuilder.UseWebApi(config);
                    });

            this.httpClient = this.httpTestServer.HttpClient;
            this.CleanDb();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            this.httpTestServer.Dispose();
        }

        [TestMethod]
        public void ListNews_EmptyDb_ShouldReturn_OkEmptyList()
        {
            var httpResponse = this.httpClient.GetAsync(NewsRequestUri).Result;
            var news = httpResponse.Content.ReadAsAsync<List<News>>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual(news.Count, 0);
        }

        [TestMethod]
        public void ListNews_NonEmptyDb_ShouldReturn_OkListNews()
        {
            var context = new NewsContext();
            var news1 = new News
            {
                Title = "Some new title 1.",
                Content = "Some freaky content.",
                PublishedDate = DateTime.Now
            };
            
            var news2 = new News
            {
                Title = "Some new title 2.",
                Content = "Some freaky content.",
                PublishedDate = DateTime.Now.AddDays(-10)
            };
            context.News.Add(news1);
            context.News.Add(news2);
            context.SaveChanges();

            var httpResponse = this.httpClient.GetAsync(NewsRequestUri).Result;
            var news = httpResponse.Content.ReadAsAsync<List<News>>().Result;
            
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(news.Count, 2);
            Assert.AreEqual(news[0].Title, news1.Title);
            Assert.AreEqual(news[1].PublishedDate.ToString(), news2.PublishedDate.ToString());
        }

        [TestMethod]
        public void CreateNews_ShouldReturn_Created()
        {
            var newsTitle = "New news title";
            var newsContent = "News content";

            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", newsTitle), 
                new KeyValuePair<string, string>("Content", newsContent)
            });

            var httpResponse = this.httpClient.PostAsync(NewsRequestUri, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(httpResponse.Headers.Location);
            Assert.AreEqual(newsFromService.Title, newsTitle);
            Assert.AreEqual(newsFromService.Content, newsContent);
            Assert.IsNotNull(newsFromService.PublishedDate);

            var context = new NewsContext();
            var newsFromDb = context.News.FirstOrDefault();
            Assert.IsNotNull(newsFromDb);
            Assert.AreEqual(newsFromDb.Id, newsFromService.Id);
            Assert.AreEqual(newsFromDb.Content, newsFromService.Content);
            Assert.AreEqual(newsFromDb.PublishedDate.ToString(), newsFromService.PublishedDate.ToString());
        }

        [TestMethod]
        public void CreateNews_ShouldReturn_BadRequest()
        {
            var newsContent = "News content";

            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Content", newsContent)
            });

            var httpResponse = this.httpClient.PostAsync(NewsRequestUri, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.BadRequest);
            Assert.IsTrue(newsFromService.Id == 0);
        }

        [TestMethod]
        public void EditExistingNews_ShouldReturn_OkEdited()
        {
            var newsTitle = "edit news title";
            var newsContent = "edit content";

            this.CreateNews_ShouldReturn_Created();
            var context = new NewsContext();
            var news = context.News.First();

            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", newsTitle), 
                new KeyValuePair<string, string>("Content", newsContent)
            });

            var httpResponse = this.httpClient.PutAsync(NewsRequestUri + "/" + news.Id, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(newsFromService.Title, newsTitle);
            Assert.AreEqual(newsFromService.Content, newsContent);
        }

        [TestMethod]
        public void EditExistingNews_WithWrongModel_ShouldReturn_BadRequest()
        {
            var newsContent = string.Empty;

            this.CreateNews_ShouldReturn_Created();
            var context = new NewsContext();
            var news = context.News.First();

            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Content", newsContent)
            });

            var httpResponse = this.httpClient.PutAsync(NewsRequestUri + "/" + news.Id, postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreNotEqual(newsFromService.Content, news.Content);
        }

        [TestMethod]
        public void EditNonExistingNews_ShouldReturn_NotFound()
        {
            var newsTitle = "edit news title";
            var newsContent = "edit content";

            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", newsTitle), 
                new KeyValuePair<string, string>("Content", newsContent)
            });

            var httpResponse = this.httpClient.PutAsync(NewsRequestUri + "/12", postContent).Result;
            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.NotFound);
            Assert.IsNull(newsFromService);
        }

        [TestMethod]
        public void DeleteExistingNews_ShouldReturn_Ok()
        {
            this.CreateNews_ShouldReturn_Created();
            var context = new NewsContext();
            var newsFromDb = context.News.First();

            var httpResponse = this.httpClient.DeleteAsync(NewsRequestUri + "/" + newsFromDb.Id).Result;
            
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteNonExistingNews_ShouldReturn_Ok()
        {
            var httpResponse = this.httpClient.DeleteAsync(NewsRequestUri + "/" + 1).Result;

            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.NotFound);
        }

        private void CleanDb()
        {
            var newsDb = new NewsContext();
            newsDb.News.Delete();

            newsDb.SaveChanges();
        }
    }
}