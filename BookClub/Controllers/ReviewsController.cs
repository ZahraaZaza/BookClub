using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookClub.Models;
namespace BookClub.Controllers
{
    public class ReviewsController : Controller
    {
        // GET: Reviews
        public ActionResult Create(int bookId)
        {
            // send users to Create action 
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "Rating, Content")] Review review, int bookId)
        {
            Review rev = new Review
            {
                BookId = bookId,
                UserName = User.Identity.Name,
                Rating = review.Rating,
                Content = review.Content
            };

            using (var db = new BooksAuthorsDB())
            {
                db.Reviews.Add(rev);
                db.SaveChanges();
            }
            return View();
        }

    }
}