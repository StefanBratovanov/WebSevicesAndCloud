using System;
using System.ComponentModel.DataAnnotations;

namespace Messages.Data.Models
{
    public class ChannelMessage
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string SenderId { get; set; }

        public virtual User Sender{ get; set; }

        public int ChannelId { get; set; }

        public virtual Channel Channel { get; set; }
    }
}
