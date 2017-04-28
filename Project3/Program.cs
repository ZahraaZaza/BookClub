
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project3
{
    class Program
    {

        public static void Main(string[] args)
        {
            XElement booksXml = XElement.Load("books.xml");
            XElement ratingsXml = XElement.Load("ratings.xml");
            createAuthors(booksXml, ratingsXml);
            //createBooks(booksXml);
           // createReviews(booksXml, ratingsXml);
            //createUsers(ratingsXml);
        }

        public static void createAuthors(XElement booksXml, XElement ratingsXml)
        {
           // int i = 0;
            List<Book> bookList = new List<Book>();
            var authors = (from book in booksXml.Descendants("book")
                          select new Author
                          {
                              FirstName = book.Element("author").Attribute("firstName")?.Value,
                              LastName = book.Element("author").Attribute("lastName")?.Value,
                          }).ToList();         
         
            var anonBooks = (from book in booksXml.Descendants("book")
                         select new
                         {
                            BookId = book.Attribute("id").Value,
                            Title = book.Element("title")?.Value,
                            Description = book.Element("description")?.Value,
                            AuthorFirst = book.Element("author").Attribute("firstName")?.Value,
                            AuthorLast = book.Element("author").Attribute("lastName")?.Value
                         });

            //Our authors have no books, but our books have authors which is an issue!!
            //So we add the books instead of the authors and it works
            foreach (var anonBook in anonBooks)
            {
                Author author = authors.Where(a => a.FirstName == anonBook.AuthorFirst
                                && a.LastName == anonBook.AuthorLast).First();

                Book book = new Book
                {
                    //BookId = int.Parse(anonBook.BookId),
                    Title = anonBook.Title,
                    Description = anonBook.Description
                };
                //book.Authors.Add(author);
                author.Books.Add(book);
                bookList.Add(book);
            }
           

            var users = from u in ratingsXml.Descendants("user")
                         select u;
            List<User> userList = new List<User>();
            foreach(var u in users)
            {
                User newUser = new User
                {
                    Username = u.Attribute("userId")?.Value,
                    Password = u.Attribute("userId")?.Value,
                    FirstName = u.Attribute("userId")?.Value,
                    LastName = u.Attribute("lastName").Value ?? "Reader",
                    Country = "CAN"
                    
                };
                userList.Add(newUser);
                foreach(var r in u.Descendants("review"))
                {
                    Review rev = new Review
                    {
                        UserName = r.Parent.Attribute("userId").Value,
                        BookId = Int32.Parse(r.Attribute("bookId").Value),
                        Rating = Int32.Parse(r.Attribute("rating")?.Value),
                        Book = bookList[Int32.Parse(r.Attribute("bookId").Value)]//.Where(x => x.BookId == Int32.Parse(r.Attribute("bookId").Value)).FirstOrDefault()
                        
                    };
                    newUser.Reviews.Add(rev);
                    //rev.Add(review);
                }

            }



            using (var db = new BooksReviewsDB())
            {
                foreach (var item in authors)
                {
                    try
                    {
                        //Console.WriteLine(item.FirstName + "\t" + item.LastName);
                        db.Authors.Add(item);
                        db.SaveChanges(); 

                    }
                    catch (DbUpdateException e)
                    {
                        db.Authors.Remove(item);
                    }
                }
                db.Users.AddRange(userList);
                db.SaveChanges(); 
            }
         
                    }
                }
               
            }
            