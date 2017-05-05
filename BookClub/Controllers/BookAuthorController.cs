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
                var listOfBook = (from Book in db.Books
                                  orderby Book.Views descending
                                  select Book).Take(10).ToList();

                return View(listOfBook);
            }
        }
        /*
         * what will be displayed after authenticated user logs in 
        public ActionResult Recommend(int? id)
        {

        }
        */

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


                return View(authorBooks);

            }
        }
    }
}