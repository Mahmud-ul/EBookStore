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
    public class PublisherController : Controller
    {
        private readonly ConnectionString _context;

        public PublisherController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Publisher
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            return View(await _context.Publishers.ToListAsync());
        }

        // GET: Publisher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // GET: Publisher/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Publisher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Status")] Publisher publisher)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            try
            {
                bool check = _context.Publishers.Any(a => a.Name == publisher.Name);
                if (check)
                {
                    ModelState.AddModelError("Name", "This publisher title already exists!");
                    return View(publisher);
                }
                else if (ModelState.IsValid)
                {
                    _context.Add(publisher);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View(publisher);
        }

        // GET: Publisher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Publisher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Status")] Publisher publisher)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id != publisher.ID)
            {
                return NotFound();
            }

            bool check = _context.Publishers.Any(a => a.Name == publisher.Name && a.ID != publisher.ID);
            if (check)
            {
                ModelState.AddModelError("Name", "This publisher title already exists!");
                return View(publisher);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.ID))
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
            return View(publisher);
        }

        // GET: Publisher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Publisher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.ID == id);
        }
    }
}
