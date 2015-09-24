using System;
using System.Linq;
using System.Web.Http;
using Messages.Data;
using Messages.Data.Models;
using Messages.RestServices.Models;
using Microsoft.AspNet.Identity;

namespace Messages.RestServices.Controllers
{
    [RoutePrefix("api/user")]
    public class UserMessagesController : ApiController
    {
        private MessagesDbContext db = new MessagesDbContext();

        [Authorize]
        [HttpGet]
        [Route("personal-messages")]
        public IHttpActionResult GetPersonalMessages()
        {
            var currentUsername = User.Identity.GetUserName();

            var messages = db.UserMessages
                .Where(m => m.RecipientUser.UserName == currentUsername)
                .OrderByDescending(m => m.DateSent)
                .Select(m => new ChannelMessageViewModel()
                {
                    Id = m.Id,
                    Text = m.Text,
                    DateSent = m.DateSent,
                    Sender = m.Sender == null ? null : m.Sender.UserName
                });

            return this.Ok(messages);
        }




        [HttpPost]
        [Route("personal-messages")]
        public IHttpActionResult SendPersonalMessage(UserMessageBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing message data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipientUser = this.db.Users.FirstOrDefault(u => u.UserName == model.Recipient);
            if (recipientUser == null)
            {
                return BadRequest("Recipient user " + model.Recipient + " does not exists.");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var message = new UserMessage()
            {
                Text = model.Text,
                DateSent = DateTime.Now,
                Sender = currentUser,
                RecipientUser = recipientUser
            };
            db.UserMessages.Add(message);
            db.SaveChanges();

            if (message.Sender == null)
            {
                return this.Ok(
                    new
                    {
                        message.Id,
                        Message = "Anonymous message sent successfully to user " + recipientUser.UserName + "."
                    }
                );
            }

            return this.Ok(
                new
                {
                    message.Id,
                    Sender = message.Sender.UserName,
                    Message = "Message sent successfully to user " + recipientUser.UserName + "."
                }
            );
        }
    }
}