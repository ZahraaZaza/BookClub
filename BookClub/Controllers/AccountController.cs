using BookClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookClub.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName, Password")] User userIn, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BooksAuthorsDB())
                {
                    //check if valid user password pair. If it is, log them into the site and return them back
                    //username is unique
                    User user = (from u in db.Users
                                 where u.Username.Equals(userIn.Username)
                                 select u).FirstOrDefault<User>();
                    if (user != null) // found a match
                        if (user.Password.Equals(userIn.Password))
                            //log into site:
                            FormsAuthentication.RedirectFromLoginPage(userIn.Username, false);

                    //Moved this from outside of the using statement to inside so i could pass the user to the view
                    //still here: either user not found, or password didn’t match
                    ViewBag.ReturnUrl = ReturnUrl;
                    ModelState.AddModelError("", "Invalid user name or password");
                    return View(user);
                }
            }
            return View();

        }
        [Authorize]
        public ActionResult Index()
        {
            using(var db = new BooksAuthorsDB())
            {
                
                var groupedUserReviews = (from r in db.Reviews
                                    where User.Identity.Name != r.UserName
                                    group r by r.UserName into groupedReviews 
                                    select groupedReviews).ToList();

                var loggedInReviews = (from r in db.Reviews
                                  where User.Identity.Name == r.UserName
                                  select r).ToList();

                int? highestRating = 0;

                string bestUser = null; 

                foreach(var user in groupedUserReviews)
                {
                    int? temp = 0;

                    foreach(var review in user)
                    {
                        int? rating = loggedInReviews.Where(x => x.BookId == review.BookId).FirstOrDefault()?.Rating;

                        if(rating != null)
                        {
                            temp += review.Rating * rating;
                        }
                    }
                    if (temp > highestRating) {
                        highestRating = temp;
                        bestUser = user.Key;
                            }

                }
                var bestBooks = (from b in db.Reviews
                                where b.UserName == bestUser
                                && b.Rating >= 0 && b.UserName != User.Identity.Name
                                select b.Book).Take(10).ToList();


                return View(bestBooks);
                //List<Review> reviews = (from)
            }
            //Show a max of 10 books
            
        }
    }
}