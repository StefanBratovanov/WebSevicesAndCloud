using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using BugTracker.Data;
using BugTracker.Data.Models;
using BugTracker.RestServices.Models.BindModels;
using BugTracker.RestServices.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace BugTracker.RestServices.Controllers
{
    [RoutePrefix("api")]
    public class CommentsController : ApiController
    {
        private BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: api/Comments
        [Route("comments")]
        public IHttpActionResult GetComments()
        {
            var comments = db.Comments
                   .OrderByDescending(c => c.PublishDate)
                   .Select(c => new CommentViewModel()
                   {
                       Id = c.Id,
                       Text = c.Text,
                       Author = c.Author == null ? null : c.Author.UserName,
                       DateCreated = c.PublishDate,
                       BugId = c.BugId,
                       BugTitle = c.Bug.Title
                   });

            return this.Ok(comments);
        }

        [HttpGet]
        [Route("bugs/{id}/comments")]
        public IHttpActionResult GetComment(int id)
        {
            var bug = db.Bugs.FirstOrDefault(x => x.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            var commnetView = bug.Comments
                .OrderByDescending(c => c.PublishDate)
                .Select(c => new CommentsViewModel()
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author == null ? null : c.Author.UserName,
                    DateCreated = c.PublishDate
                });

            return this.Ok(commnetView);
        }


        [HttpPost]
        [Route("bugs/{id}/comments")]
        public IHttpActionResult PostComment(int id, [FromBody]CommentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model == null)
            {
                return this.BadRequest("Missing comment data");
            }

            var bug = db.Bugs.Find(id);
            if (bug == null)
            {
                return this.NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);

            var commentToAdd = new Comment()
            {
                Text = model.Text,
                Author = currentUser,
                PublishDate = DateTime.Now,
                BugId = id
            };

            db.Comments.Add(commentToAdd);
            db.SaveChanges();

            if (commentToAdd.Author == null)
            {
                return this.Ok(new
                {
                    Id = commentToAdd.Id,
                    Message = "Added anonymous comment for bug # " + id
                });

            }
            return this.Ok(new
            {
                Id = commentToAdd.Id,
                Author = commentToAdd.Author.UserName,
                Message = "Added anonymous comment for bug # " + id
            });

        }







    }
}