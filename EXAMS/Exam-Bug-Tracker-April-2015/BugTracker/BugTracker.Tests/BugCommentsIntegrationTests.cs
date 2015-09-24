
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BugTracker.Data;
using BugTracker.Data.Models;
using BugTracker.RestServices;
using BugTracker.RestServices.Models.ViewModels;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;

namespace BugTracker.Tests
{
    [TestClass]
    public class BugCommentsIntegrationTests
    {
        private static TestServer httpTestServer;
        private static HttpClient httpClient;

        [AssemblyInitialize]
        public static void TestInit(TestContext context)
        {
            // Start OWIN testing HTTP server with Web API support
            httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);

                var webAppStartup = new Startup();
                webAppStartup.Configuration(appBuilder);

                appBuilder.UseWebApi(config);
            });

            httpClient = httpTestServer.HttpClient;
            Seed();
        }

        [AssemblyCleanup]
        public static void TestCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }
        }

        private static void Seed()
        {
            var context = new BugTrackerDbContext();

            if (!context.Bugs.Any())
            {
                context.Bugs.Add(new Bug()
                {
                    Title = "Bug 1",
                    Description = "First Bug",
                    SubmitDate = DateTime.Now
                });
                context.SaveChanges();

            }
        }


        [TestMethod]
        public void Get_BugComments_Should_Return_200_OK_Existing_Bug()
        {
            var context = new BugTrackerDbContext();

            var existingBug = context.Bugs.FirstOrDefault();

            if (existingBug == null)
            {
                Assert.Fail("Cannot perform test - no bug in DB");
            }

            var endpoint = string.Format("api/bugs/{0}/comments", 1);
            var responce = httpClient.GetAsync(endpoint).Result;

            Assert.AreEqual(HttpStatusCode.OK, responce.StatusCode);

            var comments = responce.Content.ReadAsAsync<List<CommentsViewModel>>().Result;

            foreach (var comment in comments)
            {
                Assert.IsNotNull(comment.Text);
                Assert.AreNotEqual(0, comment.Id);
            }
        }

        [TestMethod]
        public void Get_BugComments_Should_Return_404_Non_Existing_Bug()
        {
            var endpoint = string.Format("api/bugs/{0}/comments",-1);
            var responce = httpClient.GetAsync(endpoint).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, responce.StatusCode);
        }


    }
}
