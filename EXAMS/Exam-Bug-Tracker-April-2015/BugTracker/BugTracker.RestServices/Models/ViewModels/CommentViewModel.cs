using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BugTracker.Data.Models;

namespace BugTracker.RestServices.Models.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }

        public int BugId { get; set; }

        public string BugTitle { get; set; }
    }
}