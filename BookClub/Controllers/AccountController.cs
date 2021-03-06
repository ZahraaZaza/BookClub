﻿using BookClub.Models;
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

                    ViewBag.ReturnUrl = ReturnUrl;
                    ModelState.AddModelError("", "Invalid user name or password");
                    return View(user);
                }
            }
            return View();      
        }

        /// <summary>
        /// Gets the Register User view.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult RegisterUser(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        /// <summary>
        /// This method registers a new user using the information
        /// that the user inputed in the form.
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser([Bind(Include = "UserName, Password, LastName, FirstName, Email, Country")] User newUser, string ReturnUrl)
        {
            using (var db = new BooksAuthorsDB())
            {
                // checking to see if username already exists
                int users = (from u in db.Users
                             where u.Username.Equals(newUser.Username)
                             select u).Count();

                if (users > 0)
                {
                    ModelState.AddModelError("", "Username already exists!");
                    return View();
                }

                // creating new user
                User user = new User
                {
                    Username = newUser.Username,
                    Password = newUser.Password,
                    LastName = newUser.LastName,
                    FirstName = newUser.FirstName,
                    Email = newUser.Email,
                    Country = newUser.Country
                };

                ViewBag.ReturnUrl = ReturnUrl;
                FormsAuthentication.RedirectFromLoginPage(user.Username, false);
                db.Users.Add(user);
                db.SaveChanges();
                Login(user, ReturnUrl);
            }
            return View();
        }
        /// <summary>
        /// Logout method that logs out the authenticated users.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "BookAuthor");
        }

        [Authorize]
        public ActionResult Index()
        {
            using(var db = new BooksAuthorsDB())
            {

                // select the number of reviews that a user has
                int numOfRevs = (from r in db.Reviews
                                 where r.UserName.Equals(User.Identity.Name)
                                 select r).Count();

                /* if the user has no reviews, it means that they are new 
                   so we will display the home page since they wont have 
                   any recommended books 
                */
                if (numOfRevs == 0)
                    return RedirectToAction("Index", "BookAuthor");

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
            }        
        }
    }
}