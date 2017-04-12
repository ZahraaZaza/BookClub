using System;
using System.Collections.Generic;
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
            createAuthors(booksXml);
            //createBooks(booksXml);
           // createUsers(ratingsXml);
        }

        public static void createAuthors(XElement booksXml)
        {
            int i = 0;
            List<Book> bookList = new List<Book>();
            var authors = from book in booksXml.Descendants("book")
                          select new Author
                          {
                              FirstName = book.Element("author").Attribute("firstName")?.Value,
                              LastName = book.Element("author").Attribute("lastName")?.Value,
                          };         
         
            var anonBooks = (from book in booksXml.Descendants("book")
                         select new
                         {
                           // BookId = book.Attribute("id").Value,
                            Title = book.Element("title")?.Value,
                            Description = book.Element("description")?.Value,
                            AuthorFirst = book.Element("author").Attribute("firstName")?.Value,
                            AuthorLast = book.Element("author").Attribute("lastName")?.Value
                         });

            foreach (var anonBook in anonBooks)
            {
                Author author = authors.Where(a => a.FirstName == anonBook.AuthorFirst
                                && a.LastName == anonBook.AuthorLast).First();

                Book book = new Book
                {
                    Title = anonBook.Title,
                    Description = anonBook.Description
                };
             
                author.Books.Add(book);
                bookList.Add(book);


            }
            
            using (var db = new BooksReviewsDB())
            {
                
            }
            


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
        }
        public static void createUsers(XElement ratingsXml)
        {
            var reviews = from review in ratingsXml.Descendants("user")
                          select new
                          {
                              UserName = review.Attribute("userId")?.Value,
                              Password = review.Attribute("userId")?.Value,
                              FirstName = review.Attribute("userId")?.Value,
                              LastName = review.Attribute("lastName")?.Value
                          };

            foreach(var review in reviews)
            {
                Console.WriteLine("userID: " + review.UserName);
            }
            Console.ReadKey();
        }
        public static void createReviews(XElement booksXml, XElement ratingsXml)
        {

        }
    }
}
