using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using News.Data.Repositories;
using News.Models;

namespace _03.Unit_Tests_API_Controllers
{
    public class MockContainer
    {
        public Mock<INewsRepository<News.Models.News>> NewsRepositoryMock { get; set; }

        public Mock<INewsRepository<ApplicationUser>> UserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetUpFakeNews();

            this.SetUpFakeUsers();
        }

        private void SetUpFakeNews()
        {
            var fakeNews = new List<News.Models.News>()
            {
                new News.Models.News()
                {
                    Id = 1,
                    Title = "Una notizia nuova",
                    Content = "Non è una notizia nuova ma è sicuramente rivoluzionaria perché sovverte false certezze e manda in crisi alcuni postulati pseudo-scientifici.",
                    PublishDate = new DateTime(2015, 04, 07, 12, 45, 23)
                },
                 new News.Models.News()
                {
                    Id = 2,
                    Title = "Adobe prepara una nuova app Photoshop per iOS",
                    Content = "Photoshop per dispositivi mobili sta per cambiare.",
                    PublishDate = DateTime.Now.AddDays(-9)
                },
                new News.Models.News()
                {
                    Id = 3,
                    Title = "Nasa, scoperta una 'nuova Terra'",
                    Content = "La possibile 'nuova Terra' e' stata chiamata \"Kepler 452B\".",
                    PublishDate = DateTime.Now.AddDays(-5)
                },
                new News.Models.News()
                {
                    Id = 4,
                    Title = "Italia colpita dal maltempo Forti piogge a Milano e Roma",
                    Content = "A Milano ha iniziato a cadere molto forte dalle prime ore del mattino.",
                    PublishDate = DateTime.Now.AddDays(-50)
                }
            };
            this.NewsRepositoryMock=new Mock<INewsRepository<News.Models.News>>();
            this.NewsRepositoryMock.Setup(r => r.All()).Returns(fakeNews.AsQueryable());
            this.NewsRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns((int id) => fakeNews.FirstOrDefault(n => n.Id == id));
            this.NewsRepositoryMock.Setup(r => r.Delete(It.IsAny<News.Models.News>()))
                .Callback((News.Models.News news) =>
                {
                    fakeNews.RemoveAll(e => e.Title == news.Title);
                });
        }

        private void SetUpFakeUsers()
        {
            var users = new List<ApplicationUser>()
            {
               new ApplicationUser()
                  {
                  Id = "1",
                  UserName = "Ivan"
                  },
                  new ApplicationUser()
                  {
                  Id = "2",
                  UserName = "Petar"
                  },
                  new ApplicationUser()
                  {
                  Id = "3",
                  UserName = "Stoyan"
                  }
            };
            this.UserRepositoryMock = new Mock<INewsRepository<ApplicationUser>>();

            this.UserRepositoryMock.Setup(r => r.All()).Returns(users.AsQueryable());

            this.UserRepositoryMock.Setup(r => r.Find(It.IsAny<string>()))
                .Returns((string id) => users.FirstOrDefault(user => user.Id == id));
        }
    }
}

