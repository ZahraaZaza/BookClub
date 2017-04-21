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
                var listOfBookAuthor = (from Book in db.Books
                                        orderby Book.BookId
                                        select Book).ToList<Book>(); 

                return View(listOfBookAuthor);
            }
        }
        public ActionResult Details(int? id = 0)
        {

            int someID = 0;

            if (id.HasValue)
                someID = id.Value;

            using (var db = new BooksAuthorsDB())
            {
                var bookInfo = (from Book in db.Books
                                    where Book.BookId.Equals(someID)
                                    select Book).ToList<Book>();
              

                var bookAuthor = (from Author in db.Authors
                                  where Author.Books.Contains(bookInfo.FirstOrDefault())
                                  select Author).FirstOrDefault();
                                  
                ViewBag.BookTitle = bookInfo.FirstOrDefault().Title;
                ViewBag.BookDescription = bookInfo.FirstOrDefault().Description;
                ViewBag.BookReview = bookInfo.FirstOrDefault().Reviews;

                ViewBag.BookAuthor = bookAuthor.FirstName + " " + bookAuthor.LastName;

                return View(someID);
            }
        }
    }
}