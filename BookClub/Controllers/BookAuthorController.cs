using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookClub.Models;

namespace BookClub.Controllers
{
    public class BookAuthorController : Controller
    {
        // GET: BookAuthor
        public ActionResult Index(int? id)
        {
            using (var db = new BooksAuthorsDB())
            {
                var listOfBooks = (from Book in db.Books
                                   orderby Book.Views descending
                                   select Book).Take(10).ToList();

                return View(listOfBooks);
            }
        }

        public ActionResult BookDetails(int? id = 0)
        {
            int someID = 0;

            if (id.HasValue)
                someID = id.Value;

            using (var db = new BooksAuthorsDB())
            {
                Book bookInfo = (from Book in db.Books.Include("Reviews").Include("Authors")
                                 where Book.BookId.Equals(someID)
                                 select Book).FirstOrDefault();

                ICollection<Review> reviews = db.Books.Find(bookInfo.BookId).Reviews;

                int rating = 0;
                int sum = 0;
                int counter = 0;

                foreach (Review r in reviews)
                {
                    if (r.Rating == -5)
                        rating = 1;

                    else if (r.Rating == -3)
                        rating = 2;

                    else if (r.Rating == 0)
                        rating = 3;

                    else if (r.Rating == 3)
                        rating = 4;

                    else if (r.Rating == 5)
                        rating = 5;

                    sum += rating;
                    counter++;
                }

                ViewBag.AvgRating = sum / counter;

                bookInfo.Views += 1;
                db.SaveChanges();
                return View(bookInfo);
            }
        }

        public ActionResult AuthorDetails(int id)
        {
            using (var db = new BooksAuthorsDB())
            {
                ICollection<Book> authorBooks = db.Authors.Find(id).Books;
                // not sure
                Author a = db.Authors.Find(id);
                ViewBag.AuthorName = a.FirstName + " " + a.LastName;

                return View(authorBooks);

            }
        }

        /*
        [HttpPost]
        [Authorize]
        public ActionResult AddAuthor([Bind(Include = "FirstName, LastName")] Author author)
        {

        }*/

        /* [HttpPost]
           [Authorize]
           public ActionResult AddBook([Bind(Include = "FirstName, LastName")] Author author)
           {

           }*/
    }
}