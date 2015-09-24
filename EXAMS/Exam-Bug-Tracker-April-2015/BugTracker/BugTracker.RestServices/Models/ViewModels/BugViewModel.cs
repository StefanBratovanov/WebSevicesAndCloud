using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BugTracker.Data.Models;

namespace BugTracker.RestServices.Models.ViewModels
{
    public class BugViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

