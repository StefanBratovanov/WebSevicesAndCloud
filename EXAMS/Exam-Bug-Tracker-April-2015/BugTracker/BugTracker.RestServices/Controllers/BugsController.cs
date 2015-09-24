
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BugTracker.Data;
using BugTracker.Data.Models;
using BugTracker.RestServices.Models.BindModels;
using BugTracker.RestServices.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace BugTracker.RestServices.Controllers
{
    [RoutePrefix("api/bugs")]
    public class BugsController : ApiController
    {
        private BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: api/Bugs
        [HttpGet]
        public IHttpActionResult GetBugs()
        {
            var bugs = db.Bugs
                .OrderByDescending(b => b.SubmitDate)
                .Select(b => new BugViewModel()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Status = b.Status.ToString(),
                    Author = b.Author == null ? null : b.Author.UserName,
                    DateCreated = b.SubmitDate
                });

            return this.Ok(bugs);

        }

        // GET: api/Bugs/5
        [HttpGet]
        [ResponseType(typeof(Bug))]
        [Route("{id}")]
        public IHttpActionResult GetBug(int id)
        {
            var bug = db.Bugs.FirstOrDefault(x => x.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            var bugView = new BugFullViewModel()
            {
                Id = bug.Id,
                Title = bug.Title,
                Description = bug.Description,
                Status = bug.Status.ToString(),
                Author = bug.Author == null ? null : bug.Author.UserName,
                DateCreated = bug.SubmitDate,
                Comments = bug.Comments
                        .OrderByDescending(c => c.PublishDate)
                        .ThenByDescending(c => c.Id)
                        .Select(c => new CommentsViewModel()
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author == null ? null : c.Author.UserName,
                    DateCreated = c.PublishDate
                })
            };

            return this.Ok(bugView);
        }

        // POST: api/Bugs
        [HttpPost]
        [ResponseType(typeof(Bug))]
        public IHttpActionResult PostBug([FromBody]BugBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model == null)
            {
                return this.BadRequest("Missing bug data");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);

            var bug = new Bug()
            {
                Title = model.Title,
                Description = model.Description,
                Status = BugStatus.Open,
                Author = currentUser,
                SubmitDate = DateTime.Now
            };

            db.Bugs.Add(bug);
            db.SaveChanges();

            //if (currentUserId == null)
            if (bug.Author == null)
            {
                return this.CreatedAtRoute(
                                "DefaultApi",
                                new { controller = "bugs", id = bug.Id },
                                new
                                {
                                    Id = bug.Id,
                                    Message = "Anonymous bug submitted."
                                });

            }
            return this.CreatedAtRoute(
                                "DefaultApi",
                                new { controller = "bugs", id = bug.Id },
                                new
                                {
                                    Id = bug.Id,
                                    Author = bug.Author.UserName,
                                    Message = "User bug submitted."
                                });

        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult EditChannel(int id, [FromBody]BugPatchBinfingModel model)
        {
            var bug = db.Bugs.Find(id);

            if (bug == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Missing bug data.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (model.Title != null)
            {
                bug.Title = model.Title;
            }

            if (model.Description != null)
            {
                bug.Description = model.Description;
            }
            if (model.Status != null)
            {
                BugStatus bugStatus;

                // Call Enum.TryParse method.
                if (Enum.TryParse(model.Status, out bugStatus))
                {
                    bug.Status = bugStatus;
                }
                else
                {
                    return this.BadRequest();
                }
            }
            db.SaveChanges();
            return this.Ok(new
            {
                Message = "Bug #" + id + " patched."
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteChannelById(int id)
        {
            var bug = db.Bugs.Find(id);

            if (bug == null)
            {
                return this.NotFound();
            }

            db.Bugs.Remove(bug);
            db.SaveChanges();

            return this.Ok(new
            {
                Message = "Bug #" + id + " deleted"
            });
        }

        [HttpGet]
        [Route("filter")]
        public IHttpActionResult GetBugsByFilter(
            [FromUri] string keyword = null,
            [FromUri] string statuses = null,
            [FromUri] string author = null)
        {
            var bugs = db.Bugs
                .OrderByDescending(b => b.SubmitDate)
                .Select(b => new BugViewModel()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Status = b.Status.ToString(),
                    Author = b.Author == null ? null : b.Author.UserName,
                    DateCreated = b.SubmitDate
                }).AsQueryable();


            if (keyword != null)
            {
                bugs = bugs.Where(b => b.Title.Contains(keyword));
            }
            if (statuses != null)
            {
                var statusArray = statuses.Split('|');
                bugs = bugs.Where(b => statusArray.Contains(b.Status));
            }
            if (author != null)
            {
                bugs = bugs.Where(b => b.Author == author);
            }

            return this.Ok(bugs);
        }

    }
}