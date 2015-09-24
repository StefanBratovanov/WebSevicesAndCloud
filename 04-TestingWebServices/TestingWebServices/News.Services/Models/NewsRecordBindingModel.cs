using System;
using System.ComponentModel.DataAnnotations;

namespace News.Services.Models
{
    public class NewsRecordBindingModel
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }

    }
}