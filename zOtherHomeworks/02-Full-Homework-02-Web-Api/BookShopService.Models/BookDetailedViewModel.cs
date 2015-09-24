using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class BookDetailedViewModel
    {
        public BookDetailedViewModel(Book book)
        {
            this.Id = book.Id;
            this.Title = book.Title;
            this.Desciption = book.Desciption;
            this.Edition = book.Edition;
            this.Price = book.Price;
            this.Copies = book.Copies;
            this.AuthorId = book.AuthorId;
            this.AuthorName = book.Author.FirstName + " " + book.Author.LastName;
            this.Categories = book.Categories.Select(c => c.Name).ToList();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Desciption { get; set; }

        public Edition Edition { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public virtual List<string> Categories { get; set; }
    }
}
