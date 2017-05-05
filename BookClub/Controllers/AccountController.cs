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
                }
            }
            //still here: either user not found, or password didn’t match
            ViewBag.ReturnUrl = ReturnUrl;
            ModelState.AddModelError("", "Invalid user name or password");
            return View();

        }
    }
}