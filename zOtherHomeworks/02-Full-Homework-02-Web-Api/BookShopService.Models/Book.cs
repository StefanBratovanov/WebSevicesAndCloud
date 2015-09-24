﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    public class Book
    {
        private ICollection<Category> categories;

        public Book()
        {
            this.categories = new HashSet<Category>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Desciption { get; set; }

        public Edition Edition { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public virtual ICollection<Category> Categories
        {
            get { return this.categories; }
            set { this.categories = value; }
        }
    }
}
