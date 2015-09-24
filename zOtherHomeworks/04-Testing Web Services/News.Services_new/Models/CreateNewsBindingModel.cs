using System.ComponentModel.DataAnnotations;

namespace News.Services_new.Models
{
    public class CreateNewsBindingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

    }
}