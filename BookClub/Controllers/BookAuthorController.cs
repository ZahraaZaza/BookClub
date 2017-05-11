using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookClub.Models;

namespace BookClub.Controllers
{
    /// <summary>
    /// This controller takes care of sending the right
    /// books and authors information to the views.
    /// </summary>
    public class BookAuthorController : Controller
    {
        /// <summary>
        /// We are sending a list of 10 books to the View, which is also the 
        /// home page. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: BookAuthor
        public ActionResult Index(int? id)
        {
            using (var db = new BooksAuthorsDB())
            {
                // getting first 10 books based on the views
                var listOfBooks = (from Book in db.Books
                                   orderby Book.Views descending
                                   select Book).Take(10).ToList();
           
                return View(listOfBooks);
            }
        }
        /// <summary>
        /// When a book gets clicked, its id gets passed to this method 
        /// which sends a book object to the view  
        /// in order to display the details about that book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BookDetails(int id)
        {

            using (var db = new BooksAuthorsDB())
            {
                // getting the book object that user has chosen
                Book bookInfo = (from Book in db.Books.Include("Reviews").Include("Authors")
                                 where Book.BookId.Equals(id)
                                 select Book).FirstOrDefault();

                // getting all the reviews of the book
                ICollection<Review> reviews = db.Books.Find(bookInfo.BookId).Reviews;

                int rating = 0;
                int sum = 0;
                int counter = 0;

                // changing review numbers so user can understand
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

                // average rating of the book
                ViewBag.AvgRating = sum / counter;

                bookInfo.Views += 1;
                db.SaveChanges();
                return View(bookInfo);
            }
        }

        /// <summary>
        /// In the book details view, a user can click on the author
        /// in order to get the author's details. This method will get and send
        /// all the information about an author to the view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View of the author details</returns>
        public ActionResult AuthorDetails(int id)
        {
            using (var db = new BooksAuthorsDB())
            {
                ICollection<Book> authorBooks = db.Authors.Find(id).Books;
                Author a = db.Authors.Find(id);
                // getting author name 
                ViewBag.AuthorName = a.FirstName + " " + a.LastName;

                return View(authorBooks);

            }
        }
        /// <summary>
        /// Gets the CreateAuthor view.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAuthor()
        {        
            return View();
        }
       
        /// <summary>
        /// This method allows authenticated users to create an 
        /// author. It adds the new author to the database after 
        /// making sure it is unique. 
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult CreateAuthor([Bind(Include = "FirstName, LastName")] Author author)
        {
            using (var db = new BooksAuthorsDB())
            {
                // getting number of authors that has same name 
                int authorCount = (from Author in db.Authors
                                   where (Author.FirstName.Equals(author.FirstName)) &&
                                   (Author.LastName.Equals(author.LastName))
                                   select Author).Count();
                if (authorCount > 0)
                {
                    // error message
                    ModelState.AddModelError("", "Oops! Looks like we already have this author.");
                    return View();
                }
                Author a = new Author
                {
                    LastName = author.LastName,
                    FirstName = author.FirstName
                };

                db.Authors.Add(a);
                db.SaveChanges();
            }
            return View();
        }

        public ActionResult CreateAuthor()
        {
            ViewBag.AuthorName1 = new SelectList(db.Authors, "LastName", "LastName");
            ViewBag.AuthorName2 = new SelectList(db.Authors, "LastName", "LastName");

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateBook([Bind(Include = "BookId, Title, Description")] Book book, Author LastNameX, Author LastNameY)
        {
            using (var db = new BooksAuthorsDB()) {
                Author author1 = db.Authors 
                Author author2 =
            return View();
            }
        }
    }
}