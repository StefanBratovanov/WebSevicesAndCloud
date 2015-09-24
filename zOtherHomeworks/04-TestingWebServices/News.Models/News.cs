namespace News.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class News
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "News title can't be empty.", MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime? PublishedDate { get; set; }
    }
}