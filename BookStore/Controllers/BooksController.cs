using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookStore.Services;


namespace BookStore.Models
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Books
        //[Authorize(Roles = "User")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string title, string author, string genre)
        {
            var Book = from m in _context.Books select m;

            if (title != null)
            {
                Book = Book.Where(e => e.Title.Contains(title));
            }

            if (author != null)
            {
                Book = Book.Where(e => e.Author.Contains(author));
            }

            if (genre != null)
            {
                Book = Book.Where(e => e.Genre.Contains(genre));
            }

            return View(await Book.ToListAsync());
        }

        

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Genre,Price,Author,Publisher,Quantity,ImageUrl,ReleaseDate")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Genre,Price,Author,Publisher,Quantity,ImageUrl,ReleaseDate")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        public JsonResult BestSellers()
        {
            var list =  from book in _context.Books
                        join order in _context.Orders on book.BookId equals order.BookId
                        select new { Title=book.Title,Quantity=order.Quantity };
            var result = list.GroupBy(b => b.Title).Select(t => new { id =t.Key, counter = t.Sum(u => u.Quantity) }).OrderByDescending(c=> c.counter).Take(5);
            return Json(result.ToList());
        }
        [HttpPost, ActionName("Decrease")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseQuantity(int BookId)
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book != null)
            {
                book.Quantity = book.Quantity--;
                _context.SaveChanges();
            }



            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public JsonResult Related(int? id)
        {

            //for testing only!!
            //Clear DB before retrain
            //var rows = from o in _context.ClusterResulter
            //           select o;
            //foreach (var row in rows)
            //{
            //    _context.ClusterResulter.Remove(row);
            //}
            //_context.SaveChanges();


            //Getting The detaled book
            var book = _context.Books.Find(id);
            //(cach) Check if allready have previos prediction
            var clusterResult = _context.ClusterResulter
                .Where(b => b.BookID == id)
                .FirstOrDefault();

            IQueryable<ClusterResulter> crs;


            if (clusterResult != null)
            {
                //create list of recomandation for join Recomandation 

                crs = _context.ClusterResulter.Where(b => b.ClusterRes == clusterResult.ClusterRes);
            }
            else
            {

                //Create Dataset file from all the books Using BookService
                BookService bookService = new BookService(_context);
                bookService.ReconvertAllBooks();

                //Clear DB before retrain
                var rows = from o in _context.ClusterResulter
                           select o;
                foreach (var row in rows)
                {
                    _context.ClusterResulter.Remove(row);
                }
                _context.SaveChanges();
                //_context.ClusterResulter.RemoveRange();
                //_context.SaveChanges();

                //Train Modal
                BookClustering bc = new BookClustering();


                //Get all Books
                var books = _context.Books.ToList(); ;
                //Predict for each Book and create DB ClusterResulter
                foreach (Book tmpBook in books)
                {
                    //Preparing ClusterResulter for DB
                    ClusterResulter cr = new ClusterResulter();

                    //ADding BookID to ClusterResulter
                    cr.BookID = tmpBook.BookId;

                    // Prepare BookItem as BookData (featuresSet)
                    BookData bd = bookService.CreateBookData(tmpBook);

                    //Train & Predict
                    ClusterPrediction cp = bc.Predict(bd);
                    cr.ClusterRes = Convert.ToInt32(cp.PredictedClusterId);

                    //Save Result in DB
                    _context.ClusterResulter.Add(cr);
                }
                _context.SaveChanges();

                //Get Book Prediction Class
                ClusterPrediction cp_final = bc.Predict(bookService.CreateBookData(book));
                int predId = Convert.ToInt32(cp_final.PredictedClusterId);

                //Get relevant predictions
                crs = _context.ClusterResulter
                    .Where(b => b.ClusterRes == predId);
            }

            var recomended = from bk in _context.Books
                             join cr in crs on bk.BookId equals cr.BookID
                             where cr.BookID != id
                             select new {
                                 Id =bk.BookId,
                                 Title = bk.Title,
                                 Quantity = bk.Quantity,
                                 URL = bk.ImageUrl,
                                 Result = cr.ClusterRes
                             };

            return Json(recomended);
        }
    }
}
