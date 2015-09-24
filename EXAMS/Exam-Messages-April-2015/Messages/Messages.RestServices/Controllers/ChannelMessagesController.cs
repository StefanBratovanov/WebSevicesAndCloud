using System;
using System.Linq;
using System.Web.Http;
using Messages.Data;
using Messages.Data.Models;
using Messages.RestServices.Models;
using Microsoft.AspNet.Identity;

namespace Messages.RestServices.Controllers
{
    [RoutePrefix("api/channel-messages")]
    public class ChannelMessagesController : ApiController
    {
        private MessagesDbContext db = new MessagesDbContext();


        [HttpGet]
        [Route("{channel}")]
        public IHttpActionResult GetChannelMessages(string channel, [FromUri]string limit = null)
        {
            Channel channelDB = db.Channels.FirstOrDefault(c => c.Name == channel);

            if (channelDB == null)
            {
                return NotFound();
            }

            var messages = channelDB.ChannelMessages
                .OrderByDescending(m => m.DateSent)
                .Select(m => new ChannelMessageViewModel()
                {
                    Id = m.Id,
                    Text = m.Text,
                    DateSent = m.DateSent,
                    Sender = m.Sender == null ? null : m.Sender.UserName
                });

            if (limit != null)
            {
                int limitMessages = 0;

                int.TryParse(limit, out limitMessages);
                if (limitMessages >= 1 && limitMessages <= 1000)
                {
                    messages = messages.Take(limitMessages);
                }
                else
                {
                    return this.BadRequest("Limit should be integer in range [1..1000].");
                }
            }

            return this.Ok(messages);
        }


        [HttpPost]
        [Route("{channel}")]
        public IHttpActionResult PostChannelMessages(string channel, ChannelMessageBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing message data.");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            Channel channelDB = db.Channels.FirstOrDefault(c => c.Name == channel);

            if (channelDB == null)
            {
                return NotFound();
            }


            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);

            var message = new ChannelMessage()
            {
                Text = model.Text,
                Channel = channelDB,
                DateSent = DateTime.Now,
                Sender = currentUser
            };

            db.ChannelMessages.Add(message);
            db.SaveChanges();


            if (message.Sender == null)
            {
                return this.Ok(new
                {
                    Id = message.Id,
                    Message = "Anonymous message sent to channel " + channel
                }
            );
            }

            return this.Ok(new
                {
                    Id = message.Id,
                    Sender = message.Sender.UserName,
                    Message = "Message sent to channel " + channel
                }
            );
        }

    }
}