using System.ComponentModel.DataAnnotations;

namespace BugTracker.RestServices.Models.BindModels
{
    public class BugBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}