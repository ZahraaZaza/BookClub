using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookClub.Models;
namespace BookClub.Controllers
{
    /// <summary>
    /// This controller takes care of creating reviews. 
    /// </summary>
    public class ReviewsController : Controller
    {
        /// <summary>
        /// This method gets the id of the book in order to display
        /// the review form.
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        // GET: Reviews
        public ActionResult Create(int bookId)
        {
            // send users to Create action 
            return View();
        }

        /// <summary>
        /// This method gets the information filled by the user and 
        /// adds a new review to the Review table.
        /// </summary>
        /// <param name="review"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "Rating, Content")] Review review, int bookId)
        {
            // creating review object 
            Review rev = new Review
            {
                BookId = bookId,
                UserName = User.Identity.Name,
                Rating = review.Rating,
                Content = review.Content
            };

            using (var db = new BooksAuthorsDB())
            {
                // adding user inputed review to database
                db.Reviews.Add(rev);
                db.SaveChanges();
            }
            return View();
        }

    }
}