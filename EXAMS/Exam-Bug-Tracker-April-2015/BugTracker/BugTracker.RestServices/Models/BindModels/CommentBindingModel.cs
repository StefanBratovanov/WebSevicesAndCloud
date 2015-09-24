using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BugTracker.Data.Models;

namespace BugTracker.RestServices.Models.BindModels
{
    public class CommentBindingModel
    {
        [Required]
        public string Text { get; set; }
    }
}