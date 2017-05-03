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
        public ActionResult Index()
        {
            // send users to Create action 
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "BookId, UserName, Rating, Content")] Review review)
        {
            // POST request?
            return View("hello");
        }


    }
}