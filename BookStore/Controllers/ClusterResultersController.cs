using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;

namespace BookStore.Models
{
    public class ClusterResultersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClusterResultersController(ApplicationDbContext context)
        {
            _context = context;
        }

     

        // GET: ClusterResulters
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClusterResulter.ToListAsync());
        }

        // GET: ClusterResulters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clusterResulter = await _context.ClusterResulter
                .FirstOrDefaultAsync(m => m.ClusterResulterID == id);
            if (clusterResulter == null)
            {
                return NotFound();
            }

            return View(clusterResulter);
        }

        // GET: ClusterResulters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClusterResulters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClusterResulterID,BookID,ClusterRes")] ClusterResulter clusterResulter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clusterResulter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clusterResulter);
        }



        // GET: ClusterResulters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clusterResulter = await _context.ClusterResulter.FindAsync(id);
            if (clusterResulter == null)
            {
                return NotFound();
            }
            return View(clusterResulter);
        }

        // POST: ClusterResulters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClusterResulterID,BookID,ClusterRes")] ClusterResulter clusterResulter)
        {
            if (id != clusterResulter.ClusterResulterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clusterResulter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClusterResulterExists(clusterResulter.ClusterResulterID))
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
            return View(clusterResulter);
        }

        // GET: ClusterResulters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clusterResulter = await _context.ClusterResulter
                .FirstOrDefaultAsync(m => m.ClusterResulterID == id);
            if (clusterResulter == null)
            {
                return NotFound();
            }

            return View(clusterResulter);
        }

        // POST: ClusterResulters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clusterResulter = await _context.ClusterResulter.FindAsync(id);
            _context.ClusterResulter.Remove(clusterResulter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClusterResulterExists(int id)
        {
            return _context.ClusterResulter.Any(e => e.ClusterResulterID == id);
        }
    }
}
