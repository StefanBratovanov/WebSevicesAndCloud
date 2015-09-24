using System;
using System.ComponentModel.DataAnnotations;

namespace News.Models
{
    public class NewsRecord
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [MinLength(5)]
        public string Content { get; set; }

        public DateTime? PublishDate { get; set; }

    }
}
