using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models
{
    public class ChannelBindingModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }


    }
}