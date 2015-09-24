namespace News.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using News.Models;
    using News.Repositories;

    public class MockContainer
    {
        public Mock<IRepository<News>> NewsRepositoryMock { get; set; }

        public void PrepareNews()
        {
            this.SetupNews();
        }

        private void SetupNews()
        {
            IList<News> fakeNews = new List<News>
            {
                new News
                    {
                        Id = 1,
                        Title = "Some title",
                        Content = "Some content",
                        PublishedDate = DateTime.Now
                    },
                new News
                    {
                        Id = 2,
                        Title = "Some title2",
                        Content = "Some content2",
                        PublishedDate = DateTime.Now
                    },
                new News
                    {
                        Id = 3,
                        Title = "Some title3",
                        Content = "Some content3",
                        PublishedDate = DateTime.Now
                    }
            };

            this.NewsRepositoryMock = new Mock<IRepository<News>>();
            this.NewsRepositoryMock.Setup(n => n.All())
                .Returns(fakeNews.AsQueryable());
            this.NewsRepositoryMock.Setup(n => n.Find(It.IsAny<int>()))
                .Returns((int id) => fakeNews.FirstOrDefault(n => n.Id == id));
        }
    }
}