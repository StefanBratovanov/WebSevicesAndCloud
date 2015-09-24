using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using EntityFramework.Extensions;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using News.Data;
using News.Models;
using News.Services_new;
using News.Services_new.Models;
using Newtonsoft.Json;
using Owin;

namespace _04.Integration_Tests
{
    [TestClass]
    public class IntegartionTests
    {
        private const string Username = "TestUser";
        private const string Password = "Pass123$";
        private static TestServer server;
        private static HttpClient client;
        private string accessToken;

        public string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginResponse = this.Login();
                    Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

                    var loginData = loginResponse.Content.ReadAsAsync<LoginDTO>().Result;
                    Assert.IsNotNull(loginData.Access_Token);

                    this.accessToken = loginData.Access_Token;
                }

                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void InitializeTests(TestContext context)
        {
            ClearDatabase();
            server = TestServer.Create(AppBuilder =>
            {
                var httpConfig = new HttpConfiguration();
                WebApiConfig.Register(httpConfig);
                var webApiStartup = new Startup();
                webApiStartup.Configuration(AppBuilder);
                AppBuilder.UseWebApi(httpConfig);
            });
            client = server.HttpClient;
            Seed();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (server != null)
            {
                server.Dispose();
            }
        }

        private static void Seed()
        {
            var context = new NewsContext();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);
            var user = new ApplicationUser()
            {
                UserName = "TestUser",
                Email = "test@abv.bg"
            };
            var result = userManager.CreateAsync(user, "Pass123$").Result;
            if (!result.Succeeded)
            {
                Assert.Fail(string.Join(Environment.NewLine, result.Errors));
            }
            var news = new News.Models.News()
            {
                Title = "First news",
                Content = "first news content",
                PublishDate = DateTime.Now,
                Author = user
            };
            var news2 = new News.Models.News()
            {
                Title = "Second news",
                Content = "second news content",
                PublishDate = DateTime.Now,
                Author = user
            };
            context.News.Add(news2);
            context.SaveChanges();
        }

        [TestMethod]
        public void GetAllNews_ShouldReturn200OK()
        {
            var dbContext = new NewsContext();
            var allNews = dbContext.News
                .OrderByDescending(n => n.PublishDate)
                .Select(NewsViewModel.Create)
                .Select(n => n.Title)
                .ToList();
            var response = client.GetAsync("api/news").Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var news = response.Content.ReadAsAsync<IEnumerable<NewsViewModel>>()
                .Result
                .Select(n => n.Title)
                .ToList();
            CollectionAssert.AreEqual(allNews, news);
            Assert.AreEqual(allNews.Count, news.Count());
        }

        [TestMethod]
        public void CreateNewsWithCorrectData_ShouldReturnCreated()
        {
            var dbContext = new NewsContext();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", "new news_2"), 
                new KeyValuePair<string, string>("Content", "new news content_2")
            });
            var response = this.PostNews(data);
            var createdNews = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<News.Models.News>(createdNews);
            Assert.AreEqual("new news", result.Title);
            Assert.AreEqual("new news content", result.Content);
        }

        [TestMethod]
        public void CreateNewsWithIncorrectData_ShouldReturnBadRequest()
        {
            var dbContext = new NewsContext();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", "new news")
            });
            var response = this.PostNews(data);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void ModifyNewsWithCorrectData_ShouldReturnOK_ModifyNews()
        {
            var context = new NewsContext();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", "New news"),
                new KeyValuePair<string, string>("Content", "New news content")
            });
            var news = context.News.First(n => n.Title.Contains("Second"));
            var newsId = news.Id;
            var endpoint = string.Format("api/news/{0}", newsId);
            var response = client.PutAsync(endpoint, data).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var modifiedNews = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<News.Models.News>(modifiedNews);
            Assert.AreEqual("New news", result.Title);
            Assert.AreEqual("New news content", result.Content);
        }

        [TestMethod]
        public void ModifyNewsWithIncorrectData_ShouldReturnBadRequest()
        {
            var context = new NewsContext();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", "New news")
            });
            var newsId = context.News.First(n => n.Title.Contains("Second"));
            var endpoint = string.Format("api/news/{0}", newsId);
            var response = client.PutAsync(endpoint, data).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var dbNews = context.News.FirstOrDefault(n => n.Title == "New news");
            Assert.IsNull(dbNews);
        }

        [TestMethod]
        public void DeleteNewsWhenIdIsCorrect_ShouldReturnOK_DeleteNews()
        {
            var context = new NewsContext();
            var news = context.News.First(n => n.Title.Contains("Second"));
            var newsId = news.Id;
            var endpoint = string.Format("api/news/{0}", newsId);
            var response = client.DeleteAsync(endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var dbNews = context.News.FirstOrDefault(n => n.Title.Contains("Second"));
            Assert.IsNull(dbNews);
        }

        [TestMethod]
        public void DeleteNewsWhenIdIsInvalid_ShouldReturnBadRequest()
        {
            var context = new NewsContext();
            int id = 100;
            var dbNews = context.News.FirstOrDefault(n => n.Id == id);
            var endpoint = string.Format("api/news/{0}", id);
            var response = client.DeleteAsync(endpoint).Result;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static void ClearDatabase()
        {
            var context = new NewsContext();
            context.News.Delete();
            context.Users.Delete();
            context.SaveChanges();
        }

        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Username", "TestUser"), 
                new KeyValuePair<string, string>("Password", "Pass123$"), 
                new KeyValuePair<string, string>("grant_type", "password"), 
            });

            var response = client.PostAsync("/Token", loginData).Result;

            return response;
        }

        private void AddAuthenticationToken()
        {
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders
                    .Add("Authorization", "Bearer " + this.AccessToken);
            }
        }

        private void RemoveAuthenticatioToken()
        {
            if (client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders
                    .Remove("Authorization");
            }
        }

        private HttpResponseMessage PostNews(FormUrlEncodedContent data)
        {
            this.AddAuthenticationToken();

            return client.PostAsync("/api/news", data).Result;
        }

        private HttpResponseMessage DeleteNews(int id)
        {
            this.AddAuthenticationToken();

            return client.DeleteAsync("/api/news/" + id).Result;
        }

        private HttpResponseMessage GetAllNews()
        {
            this.RemoveAuthenticatioToken();

            return client.GetAsync("/api/news/").Result;
        }
    }
}

