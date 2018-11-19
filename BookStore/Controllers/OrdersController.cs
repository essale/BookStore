using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace BookStore
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            IList<string> roles = await _userManager.GetRolesAsync(currentUser);
            bool isAdmin = false;
            foreach (var role in roles)
            {
                if (role == "Admin")
                {
                    isAdmin = true;
                }
            }
            if (isAdmin)
            {
                ViewData["readOnly"] = false;
                var applicationDbContext = _context.Orders.Include(o => o.Customer);
                return View(await applicationDbContext.ToListAsync());

            }
            else
            {
                ViewData["readOnly"] = true;
                var customer = _context.Customers.Where(c => c.EmailAddress == currentUser.Email).Select(c => c.CustomerId).First();
                var applicationDbContext = _context.Orders.Where(o => o.CustomerId == customer).Include(o => o.Customer).Include(o => o.Book);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Book)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Author");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "EmailAddress");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,BookId,Quantity,TotalPrice,Date")] Order order)
        {
            order.Customer = _context.Customers.Where(c => c.CustomerId == order.CustomerId).First();
            order.Book = _context.Books.Where(b => b.BookId == order.BookId).First();

            _context.Add(order);

            // TODO: Here should be the code to decrease the quantity 
            // of a book once it is added to the order

            // DeacreaseQuantity implemented in BooksController. 

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private class BookOrder
        {
            public string UserName { get; set; }
            public Dictionary<int, int> BookData { get; set; }
        }

        [HttpPost]
        public string Insert(string booksorder)
        {
            try
            {
                BookOrder bookorder = JsonConvert.DeserializeObject<BookOrder>(booksorder);
                DateTime now = DateTime.Now;
                foreach (int bookId in bookorder.BookData.Keys)
                {
                    Order order = new Order();
                    order.Customer = _context.Customers.Where(c => c.EmailAddress == bookorder.UserName).First();
                    order.CustomerId = order.Customer.CustomerId;
                    order.Date = now;
                    order.BookId = bookId;
                    order.Book = _context.Books.Where(b => b.BookId == bookId).First();
                    order.Quantity = bookorder.BookData[bookId];
                    order.TotalPrice = order.Book.Price * order.Quantity;
                    _context.Add(order);
                    var book = _context.Books.Find(bookId);
                    if (book != null)
                    {
                        book.Quantity = book.Quantity - order.Quantity;
                    }
                }
                    _context.SaveChanges();
                    return "saved";

               
            }
            catch (Exception e)
            {
                return "Could not save changes";
            }
            
            
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Author", order.BookId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Country", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,BookId,Quantity,TotalPrice,Date")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Author", order.BookId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Country", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Book)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        public JsonResult OrdersPerDay()
        {
            var datetimes = new List<DateTime>();
            for (int i = 0; i < 30; i++)
            {
                datetimes.Add(DateTime.UtcNow.AddDays(-i).Date);
            }
            var emptyTableQuery =
            from dt in datetimes
            select new
            {
                Day = dt.Date.ToString("yyyy-MM-dd"),
                Quantity = 0
            };
            var list = from order in _context.Orders
                       where (order.Date > DateTime.Now.AddDays(-30))
                       select new { Day = order.Date.ToString("yyyy-MM-dd"), Quantity = order.Quantity };
            var comblinedList = list.ToList().Union(emptyTableQuery.ToList());
            var result = comblinedList.GroupBy(b => b.Day).Select(t => new { id = t.Key, counter = t.Sum(u => u.Quantity) }).OrderBy(o=> o.id);
            return Json(result.ToList());
        }
    }
}
