using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MicroSocialPlatform.Models;
using Microsoft.AspNet.Identity;

namespace MicroSocialPlatform.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Posts

        public ActionResult Index()
        {
            //var posts = from post in db.Posts
            //            select post;
            var allposts = db.Posts.Include("User").ToList();


            List<Post> posts = new List<Post>();
            foreach (var post in allposts)
            {
                var publicProfile = db.Profile.Find(post.UserId).publicProfile;
                if (publicProfile == true || User.IsInRole("Admin") || post.UserId == User.Identity.GetUserId())
                {
                    posts.Add(post);
                }
                else
                {
                    if (db.Friendship.Find(post.UserId, User.Identity.GetUserId()) != null)
                        posts.Add(post);
                }
            }

            ViewBag.Posts = posts;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            return View();
        }
        //Get
        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);
            if (db.Friendship.Find(post.UserId, User.Identity.GetUserId()) != null | db.Profile.Find(post.UserId).publicProfile == true | User.IsInRole("Admin") | post.UserId == User.Identity.GetUserId())
                ViewBag.Show = true;
            else ViewBag.Show = false;
            if (!User.Identity.IsAuthenticated)
                ViewBag.Owner = false;
            else if (post.UserId == User.Identity.GetUserId() | User.IsInRole("Admin"))
                ViewBag.Owner = true;
            else ViewBag.Owner = false;

            ViewBag.Post = post;
            ViewBag.PostId = id;
            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;
            ViewBag.Comments = comments;
            return View(post);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Post post = new Post();
            post.UserId = User.Identity.GetUserId();
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Post postare)
        {
            postare.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(postare);
                    db.SaveChanges();
                    TempData["message"] = "New post added!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(postare);
                }
            }
            catch (Exception e)
            {
                return View(postare);
            }

        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            ViewBag.Post = post;

            if (post.UserId == User.Identity.GetUserId() | User.IsInRole("Admin"))
            {
                return View(post);
            }

            else
            {
                TempData["message"] = "You don't have the rights to edit this post!";
                return RedirectToAction("Index");
            }
        }
        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Post post = db.Posts.Find(id);

                    if (TryUpdateModel(post))
                    {
                        post.Content = requestPost.Content;
                        post.Date = DateTime.Now;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Show", new { id = id });
                }

                else
                {
                    return View(requestPost);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);

            if (post.UserId == User.Identity.GetUserId() | User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "The post was deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have the rights to delete this post!";
                return RedirectToAction("Index");
            }
        }



    }
}