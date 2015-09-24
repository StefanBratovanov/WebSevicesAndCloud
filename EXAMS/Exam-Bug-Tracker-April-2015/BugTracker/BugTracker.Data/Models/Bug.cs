using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Data.Models
{
    public class Bug
    {
        private ICollection<Comment> comments;

        public Bug()
        {
            this.comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public BugStatus Status { get; set; }

        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        [Required]
        public DateTime SubmitDate { get; set; }

        public virtual ICollection<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }
    }
}


//Bugs have title, description (optional), status (Open / InProgress / Fixed / Closed), author (registered user, optional)
//and submit date. Comments belong to existing bugs and have text, author and publish date.