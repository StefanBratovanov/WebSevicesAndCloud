using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messages.Data.Models
{
    public class Channel
    {

        private ICollection<ChannelMessage> channelMessages;

        public Channel()
        {
            this.channelMessages = new HashSet<ChannelMessage>();
        }

        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        [MinLength(1)]
        public string Name { get; set; }

        public virtual ICollection<ChannelMessage> ChannelMessages
        {
            get { return this.channelMessages; }
            set { this.channelMessages = value; }
        }

    }
}
