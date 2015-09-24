
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Messages.Data;
using Messages.Data.Models;
using Messages.RestServices.Models;
using Microsoft.AspNet.Identity;

namespace Messages.RestServices.Controllers
{
    public class ChannelsController : ApiController
    {

        private MessagesDbContext db = new MessagesDbContext();

        // GET: api/Channels
        [HttpGet]
        [Route("api/channels")]
        [AllowAnonymous]
        public IHttpActionResult GetAds()
        {
            var chanels = db.Channels
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name
                });

            return this.Ok(chanels);

        }
        // GET: api/Channels/5
        [ResponseType(typeof(Channel))]
        [Route("api/channels/{id}")]
        public IHttpActionResult GetChannel(int id)
        {
            Channel channel = db.Channels.FirstOrDefault(c => c.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            return this.Ok(new
            {
                channel.Id,
                channel.Name
            });
        }


        // POST: api/Channels
        [HttpPost]
        [ResponseType(typeof(Channel))]
        [Route("api/channels")]
        public IHttpActionResult PostChannel([FromBody]ChannelBindingModel model)
        {

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            if (model == null)
            {
                return this.BadRequest("Missing channel data");
            }

            var channelName = db.Channels.Any(a => a.Name == model.Name);

            if (channelName == true)
            {
                //return this.Conflict();
                return this.Content(HttpStatusCode.Conflict,
                    new { Message = "Duplicated channel name: " + model.Name });
            }


            var channelToAdd = new Channel()
            {
                Name = model.Name
            };

            db.Channels.Add(channelToAdd);
            db.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "channels", id = channelToAdd.Id },
                new { channelToAdd.Id, channelToAdd.Name });

        }

        [HttpPut]
        [Route("api/channels/{id}")]
        public IHttpActionResult EditChannel(int id, [FromBody]ChannelBindingModel model)
        {
            var channel = db.Channels.Find(id);

            if (channel == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Missing channel data.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            //  var chanelNameExists = db.Channels.Any(c => c.Name == model.Name);

            if (db.Channels.Any(c => c.Name == model.Name && c.Id != id))
            {
                return this.Content(HttpStatusCode.Conflict,
                   new { Message = "Channel name " + model.Name + " already exists: " });
            }

            channel.Name = model.Name;

            db.SaveChanges();

            //var returnData = db.Channels
            //    .Where(c => c.Id == channel.Id)
            //    .Select(c => new
            //    {
            //        c.Name
            //    })
            //    .FirstOrDefault();

            return this.Ok(new
            {
                Message = "Channel #" + id + " edited successfully."
            });
        }

        //DELETE	/api/books/{id}
        [HttpDelete]
        [Route("api/channels/{id}")]
        public IHttpActionResult DeleteChannelById(int id)
        {
            var channel = db.Channels.Find(id);

            if (channel == null)
            {
                return this.NotFound();
            }
            //if (channel.Messages.Any())
            if (db.Channels.Any(c => c.Id == id && c.ChannelMessages.Count != 0))
            {
                return this.Content(HttpStatusCode.Conflict,
                   new { Message = "Cannot delete channel #" + id + " because it is not empty." });
            }


            db.Channels.Remove(channel);
            db.SaveChanges();

            return this.Ok(new
            {
                Message = "Channel #" + id + " deleted"
            });
        }

    }
}