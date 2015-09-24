namespace News.Services.Models
{
    using System;
    using System.Linq.Expressions;

    using News.Models;

    public class NewsViewModel
    {
        public NewsViewModel()
        {
        }

        public NewsViewModel(News news)
        {
            this.Title = news.Title;
            this.Content = news.Content;
            this.PublishedDate = news.PublishedDate;
        }

        public static Expression<Func<News, NewsViewModel>> CreateView
        {
            get
            {
                return
                    news =>
                    new NewsViewModel
                        {
                            Title = news.Title,
                            Content = news.Content,
                            PublishedDate = news.PublishedDate
                        };
            }
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? PublishedDate { get; set; }
    }
}