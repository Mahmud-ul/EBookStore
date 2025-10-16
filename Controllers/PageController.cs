using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBookStore.Models;
using EBookStore.Models.Database;
using EBookStore.Models.CreateModel;

namespace EBookStore.Controllers
{
    public class PageController : Controller
    {
        private readonly ConnectionString _context;

        public PageController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Page
        public async Task<IActionResult> Index()
        {
            var connectionString = _context.Pages.Include(p => p.Product);
            return View(await connectionString.ToListAsync());
        }

        // GET: Page/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name");
            return View();
        }

        // POST: Page/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProductID,PageNumber,Image,Status")] PageCreateModel page)
        {
            try
            {
                bool check = await _context.Pages.AnyAsync(w => w.ProductID == page.ProductID && w.PageNumber == page.PageNumber);
                if (check)
                {
                    TempData["Error"] = "Page number already exists!";
                    ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", page.ProductID);
                    return View(page);
                }
                else if (ModelState.IsValid)
                {
                    string imagePath = string.Empty;

                    if (page.Image != null && page.Image.Length > 0)
                    {
                        // Set upload folder
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Page");
                        if (!Directory.Exists(uploadFolder))
                            Directory.CreateDirectory(uploadFolder);

                        // Unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(page.Image.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        // Save file to wwwroot/images/page
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            page.Image.CopyTo(stream);
                        }

                        imagePath = "/Image/page/" + fileName; // Relative path for <img src="">
                    }

                    Page page2 = new Page
                    {
                        ProductID = page.ProductID,
                        PageNumber = page.PageNumber,
                        Image = imagePath,
                        Status = page.Status
                    };

                    _context.Add(page2);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", page.ProductID);
            return View(page);
        }

        // GET: Page/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", page.ProductID);
            return View(page);
        }

        // POST: Page/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProductID,PageNumber,Status")] Page page)
        {
            if (id != page.ID)
            {
                return NotFound();
            }
            bool check = await _context.Pages.AnyAsync(w => w.ProductID == page.ProductID && w.PageNumber == page.PageNumber && w.ID != page.ID);
            if (check)
            {
                TempData["Error"] = "Page number already exists!";
                ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", page.ProductID);
                return View(page);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    string? image = await _context.Products.Where(w => w.ID == id).Select(s => s.Image).FirstOrDefaultAsync();
                    if (image != null)
                    {
                        page.Image = image;
                        _context.Update(page);
                        await _context.SaveChangesAsync();
                        int save = await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            TempData["Success"] = "Page updated successfully!";
                        }
                    }     
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.ID))
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
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", page.ProductID);
            return View(page);
        }

        public async Task<IActionResult> EditImage(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("Index", "Home");

            Page? page = await _context.Pages.Where(w=>w.ID == id).Include(i=>i.Product).FirstOrDefaultAsync();

            TempData["PageID"] = id;

            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(int id, IFormFile image)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("Index", "Home");

            try
            {
                int ID = Convert.ToInt32(TempData["PageID"]);

                if (ID != id)
                {
                    TempData["Error"] = "Page Tracking Error, Unable to update Image!!!";
                    return RedirectToAction("Index");
                }

                Page? page = await _context.Pages.FindAsync(id);

                if (page != null)
                {
                    if (!string.IsNullOrEmpty(page.Image))
                    {
                        string oldPartialPath = page.Image ?? string.Empty;
                        string oldFileName = oldPartialPath.Split('/').Last();
                        string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Page/") + oldFileName;

                        if (image != null && image.Length > 0)
                        {
                            if (System.IO.File.Exists(oldFullPath))
                            {
                                System.IO.File.Delete(oldFullPath);
                            }

                            // Unique file name
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            string FullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Page/") + fileName;

                            using (var stream = new FileStream(FullPath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }
                            string imagePath = "/Image/Page/" + fileName;
                            page.Image = imagePath;

                            _context.Update(page);
                            int save = _context.SaveChanges();

                            if (save > 0)
                            {
                                TempData["Success"] = "Image Updated Successfully!!!";
                            }
                            else
                            {
                                TempData["Error"] = "Image Update Failed!!!";
                            }
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        string imagePath = string.Empty;

                        if (image != null && image.Length > 0)
                        {
                            // Set upload folder
                            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Page");
                            if (!Directory.Exists(uploadFolder))
                                Directory.CreateDirectory(uploadFolder);

                            // Unique file name
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadFolder, fileName);

                            // Save file to wwwroot/images/page
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }

                            imagePath = "/Image/Page/" + fileName; // Relative path for <img src="">
                            page.Image = imagePath;
                            _context.Update(page);

                            int save = _context.SaveChanges();

                            if (save > 0)
                            {
                                TempData["Success"] = "Image Update Successful!!!";
                            }
                            else
                            {
                                TempData["Error"] = "Image Update Failed!!!";
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
                TempData["Error"] = "Page not found!!!";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Page/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Page/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                string oldPartialPath = page.Image ?? string.Empty;
                string oldFileName = oldPartialPath.Split('/').Last();
                string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Page/") + oldFileName;
                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Delete(oldFullPath);
                }

                _context.Pages.Remove(page);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.ID == id);
        }
    }
}
