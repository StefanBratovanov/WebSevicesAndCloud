using System;
using System.Collections.Generic;

namespace Messages.Data.Models
{
    public class UserMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public int SenderId { get; set; }

        public virtual User Sender { get; set; }

        public int RecipientUserId { get; set; }

        public virtual User RecipientUser { get; set; }

    }
}
