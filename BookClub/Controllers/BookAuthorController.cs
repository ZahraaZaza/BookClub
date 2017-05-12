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
        //var db = new BooksAuthorsDB();
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

        /// <summary>
        /// This is the GET to create a book.It is responsible
        /// for sending the select list through a viewbag to the POST
        /// CreateBook method
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateBook()
        {
            
            using (var db = new BooksAuthorsDB())
            {


                List<Author> allAuthors = (from a in db.Authors select a).ToList<Author>();
                var selectList = new List<SelectListItem>();
                foreach(var author in allAuthors)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = author.AuthorId.ToString(),
                        Text = author.FirstName + " " + author.LastName

                    });
                }
                ViewBag.Author1 = selectList;
                ViewBag.Author2 = selectList;


            }
            return View();
        }

        /// <summary>
        /// This is the GET of the create a book action. Allowing 
        /// a user who is logged in to add a book
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult CreateBook([Bind(Include = "BookId, Title, Description")] Book book, string author1, string author2)
        {
            using (var db = new BooksAuthorsDB()) {
                int id1 = Int32.Parse(author1);
                int id2 = Int32.Parse(author2);
                Author auth1 = db.Authors.Where(a => a.AuthorId == id1).FirstOrDefault();
                Author auth2 = db.Authors.Where(a => a.AuthorId == id2).FirstOrDefault();
                Book title = db.Books.Where(t => t.Title == book.Title).FirstOrDefault();

                if (title == null)
                {
                    Book thisBook = new Book();
                    thisBook.Title = book.Title;
                    thisBook.Description = book.Description;

                    thisBook.Authors.Add(auth1);
                    thisBook.Authors.Add(auth2);
                    auth1.Books.Add(thisBook);
                    auth2.Books.Add(thisBook);

                    db.Books.Add(thisBook);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "The book entered is already in the Book Club");
           
            }
            return View();
        }
        
    }
}