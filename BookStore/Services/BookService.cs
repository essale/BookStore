using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Data;
using System.IO;

namespace BookStore.Services
{    
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Book newBook)
        {
            _context.Add(newBook);
            _context.SaveChanges();
        }

        public Book Get(int id)
        {
            return GetAll().FirstOrDefault(book => book.BookId == id);
        }

        public IEnumerable<Book> GetAll()
        {
            return _context.Books;
        }

        public IEnumerable<Book> GetByAuthor(string author)
        {
            return _context.Books.Where(book => book.Author.Contains(author));
        }

        public void ReconvertAllBooks()
        {
            if (File.Exists(@"booksDataset.csv"))
            {
                File.Delete(@"booksDataset.csv");
            }
            var books = GetAll();
            foreach(Book book in books)
            {
                SaveFeaturesSetToDatasetFile(ConvertBookToFeaturesArray(book));
            }

        }

        //Save book as featuresSet to bookDataset.csv file
        public void SaveFeaturesSetToDatasetFile(string featuresSet)
        {
            using (StreamWriter writer = File.AppendText("booksDataset.csv")) 
            {
                writer.WriteLine(featuresSet);
            }
        }

        public string ConvertBookToFeaturesArray(Book book)
        {
            int genre = GetGenre(book.Genre);
            int date = GetDate(book.ReleaseDate);
            int price = book.Price;
            //[ genre, date , price ] featuresSet
            int[] temp = { genre, date , price};
            return string.Join(",", temp);
        }

        //return int of datetime diffrence
        private int GetDate(DateTime oldDate)
        {
            return 1 / (DateTime.Now.Millisecond - oldDate.Millisecond) ;
        }

        //Using the system genre to create int for vector
        private int GetGenre(string genreName)
        {
            int genreId = 0;
            switch (genreName)
            {
                case "Biography": genreId = 1; break;
                case "Baby": genreId = 2; break;
                case "Fiction": genreId = 3; break;
                case "Science Fiction": genreId = 4; break;
                case "Thriller": genreId = 5; break;
                case "History": genreId = 6; break;
                case "Psychology": genreId = 7; break;
                case "Science": genreId = 8; break;
                case "Romance": genreId = 9; break;
                case "Horror": genreId = 10; break;
                case "Classic": genreId = 11; break;
            }
            return genreId;
        }

        public BookData CreateBookData(Book book)
        {
            // Prepare BookItem as BookData (featuresSet)
            string convertedData = ConvertBookToFeaturesArray(book);
            List<string> BookFeaturesSet = convertedData.Split(',').ToList();
            return new BookData
            {
                genre = float.Parse(BookFeaturesSet[0]), //book.Genre,
                releaseTime = float.Parse(BookFeaturesSet[1]),
                price = float.Parse(BookFeaturesSet[2])//book.Price
            };
             
        }

    }
}
