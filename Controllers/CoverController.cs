using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBookStore.Models;
using EBookStore.Models.Database;

namespace EBookStore.Controllers
{
    public class CoverController : Controller
    {
        private readonly ConnectionString _context;

        public CoverController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Cover
        public async Task<IActionResult> Index()
        {
            return View(await _context.Covers.ToListAsync());
        }

        // GET: Cover/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cover = await _context.Covers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(cover);
        }

        // GET: Cover/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cover/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Status")] Cover cover)
        {
            try
            {
                bool check = _context.Covers.Any(a => a.Name == cover.Name);
                if (check)
                {
                    ModelState.AddModelError("Name", "This cover title already exists!");
                    return View(cover);
                }
                else if (ModelState.IsValid)
                {
                    _context.Add(cover);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View(cover);
        }

        // GET: Cover/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cover = await _context.Covers.FindAsync(id);
            if (cover == null)
            {
                return NotFound();
            }
            return View(cover);
        }

        // POST: Cover/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Status")] Cover cover)
        {
            if (id != cover.ID)
            {
                return NotFound();
            }
            bool check = _context.Covers.Any(a => a.Name == cover.Name && a.ID != cover.ID);
            if (check)
            {
                ModelState.AddModelError("Name", "This cover title already exists!");
                return View(cover);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cover);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoverExists(cover.ID))
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
            return View(cover);
        }

        // GET: Cover/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cover = await _context.Covers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(cover);
        }

        // POST: Cover/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cover = await _context.Covers.FindAsync(id);
            if (cover != null)
            {
                _context.Covers.Remove(cover);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoverExists(int id)
        {
            return _context.Covers.Any(e => e.ID == id);
        }
    }
}
