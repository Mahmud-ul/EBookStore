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
    public class AuthorController : Controller
    {
        private readonly ConnectionString _context;

        public AuthorController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Author
        public async Task<IActionResult> Index()
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Admin" || HttpContext.Session.GetString("UserType") == "Viewer") { }
            else
=======
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            return View(await _context.Authors.ToListAsync());
        }

        // GET: Author/Details/5
        public async Task<IActionResult> Details(int? id)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Admin" || HttpContext.Session.GetString("UserType") == "Viewer") { }
            else
=======
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Author/Create
        public IActionResult Create()
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Admin" || HttpContext.Session.GetString("UserType") == "Viewer") { }
            else
=======
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Author/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Status")] Author author)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Viewer")
            {
                TempData["Error"] = "This ID is for view only!!!";
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("UserType") != "Admin")
=======
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            try
            {
                bool check = _context.Authors.Any(a => a.Name == author.Name);
                if (check)
                {
                    ModelState.AddModelError("Name", "This author name already exists!");
                    return View(author);
                }
                else if (ModelState.IsValid)
                {
                    _context.Add(author);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }               
            }
            catch (Exception ex) 
            {
                TempData["Error"] = ex.Message;
            }
            return View(author);
        }

        // GET: Author/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Author/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Status")] Author author)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Viewer")
            {
                TempData["Error"] = "This ID is for view only!!!";
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("UserType") != "Admin")
=======
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            if (id != author.ID)
            {
                return NotFound();
            }

            bool check = _context.Authors.Any(a => a.Name == author.Name && a.ID != author.ID);
            if (check)
            {
                ModelState.AddModelError("Name", "This author name already exists!");
                return View(author);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.ID))
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
            return View(author);
        }

        // GET: Author/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Admin" || HttpContext.Session.GetString("UserType") == "Viewer") { }
            else
=======
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("UserType") == "Viewer")
            {
                TempData["Error"] = "This ID is for view only!!!";
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("UserType") != "Admin")
=======
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641
                return RedirectToAction("Index", "Home");

            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.ID == id);
        }
    }
}
