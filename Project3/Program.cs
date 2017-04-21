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
            /*
            // for testing purposes 
            int i = 0;
            foreach(var book in bookList)
            {
                Console.WriteLine("i "+ i);
                Console.WriteLine(book.BookId);
                i++;
            }

            Console.ReadKey();
            */

            
           /* using (var db = new BooksReviewsDB())
            {
                foreach (var item in authors)
                {
                    try
                    {
                        //Console.WriteLine(item.FirstName + "\t" + item.LastName);
                        db.Authors.Add(item);
                        //db.SaveChanges(); 

                    }
                    catch (DbUpdateException e)
                    {
                        db.Authors.Remove(item);
                    }
                }
            }*/


            /* foreach (var author in authors)
             {
                 Console.WriteLine("id " + i);
                 Console.WriteLine(author.firstname + " " + author.lastname);
                 i++;
             }
             */

            /*foreach (var book in anonBooks)
            {
                //  Console.WriteLine("id " + book.BookId);
                Console.WriteLine("id\t" + i);
                Console.WriteLine("Title:\t" + book.Title);
                Console.WriteLine("Description:\t" + book.Description);
                //??
                Console.WriteLine("Author:\t" + book.AuthorFirst + " " + book.AuthorLast + "\n");
                i++;

            }
            Console.ReadKey();
            */

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
            /*Username = review.Attribute("userId")?.Value,
            Password = review.Attribute("userId")?.Value,
            FirstName = review.Attribute("userId")?.Value,
            LastName = review.Attribute("lastName").Value ?? "Reader",
            Country = "CAN"

        }).ToList();*/
            /*
            foreach(var user in users)
            {
                Console.WriteLine("userID:\t" + user.UserName + "\npassword:\t" + user.Password);
                Console.WriteLine("First Name:\t" + user.FirstName + "\nLast Name:\t" + user.LastName);
                Console.WriteLine("Country:\t" + user.Country + "\n");
            }
            Console.ReadKey();
            */
            //List<Review> rev = new List<Review>();
            /*IEnumerable<XElement> xRating = ratingsXml.Descendants("user");
            var ratings = xRating.Descendants().Where(x => x.Attribute("rating") != null);
            var reviews = (from review in ratings
                           select new Review
                          {
                              UserName = review.Parent.Attribute("userId").Value,
                              BookId = Int32.Parse(review.Attribute("bookId").Value),      
                              Rating = Int32.Parse(review.Attribute("rating")?.Value)
                          }).ToList();*/
            /*
            foreach (var r in reviews)
            {
                Console.WriteLine(r.UserName + " " + r.BookId + " " + r.Rating);
            }*/
           /* foreach(var b in bookList)
            {
                foreach (var r in reviews)
                {
                    if (b.BookId == r.BookId)
                    {
                        Console.WriteLine(b.BookId);*/
                        /*
                        User user = users.Where(x => x.Username == r.UserName).FirstOrDefault();

                        Review review = new Review();
                        review.BookId = r.BookId;
                        review.UserName = r.UserName;
                        review.Rating = r.Rating;
                        review.Content = "";

                        user.Reviews.Add(review);

                        Console.WriteLine(user.FirstName + " " + review.UserName);
                        //rev.Add(review);*/
                    }
                }
                //Console.WriteLine(b.Reviews);
               // Console.ReadKey();
            }
            /*
            using (var db = new BooksReviewsDB())
            {
                  db.Users.AddRange(users);
                  db.SaveChanges();               
            }
            
            /* foreach (var review in reviews)
             {
                 Console.WriteLine("user:\t" + review.UserId);
                 Console.WriteLine("book id:\t" + review.BookId);
                 Console.WriteLine("Book object:\t" + review.BookTitle.Title);
                 //Console.WriteLine("title:\t" + bookList[int.Parse(review.BookId)].Title);
                 Console.WriteLine("Rating:\t" + review.Rating + "\n");
             }
             Console.ReadKey();

     */
        
       /* public static void createReviews(XElement booksXml, XElement ratingsXml)
        {
            IEnumerable<XElement> xRating = ratingsXml.Descendants("user");
            var ratings = xRating.Descendants().Where(x => x.Attribute("rating") != null);
            var reviews = from review in ratings
                          select new
                          {
                              UserId = review.Parent.Attribute("userId").Value,
                              BookId = review.Attribute("bookId").Value,      
                              Rating = review.Attribute("rating")?.Value
                          };
           
            foreach(var review in reviews)
            {
                Console.WriteLine("user:\t" + review.UserId);
                Console.WriteLine("book id:\t" + review.BookId);
                Console.WriteLine("Rating:\t" + review.Rating + "\n");
            }
            Console.ReadKey();
        }
    }*/
//}
