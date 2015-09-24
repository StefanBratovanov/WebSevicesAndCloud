using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public int BugId { get; set; }

        [Required]
        public virtual Bug Bug { get; set; }
    }
}
//Comments belong to existing bugs and have text, author and publish date.