using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Messages.Data.Models;

namespace Messages.RestServices.Models
{
    public class ChannelMessageViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string Sender { get; set; }
    }
}