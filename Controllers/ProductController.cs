using EBookStore.Models;
using EBookStore.Models.CreateModel;
using EBookStore.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ConnectionString _context;

        public ProductController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var connectionString = _context.Products.Include(p => p.Author).Include(p => p.Category).Include(p => p.Cover).Include(p => p.Publisher);
            return View(await connectionString.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Cover)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name");
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w=>w.MainCatID!=null && w.Status), "ID", "Name");
            ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name");
            ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Image,CategoryID,AuthorID,PublisherID,CoverID,Price,Discount,PageQuantity,Topic,Description,Featured,New,PreOrderable,InStock,BestSeller,SlideShow,Status")] ProductCreateModel product)
        {
            try
            {
                bool check = _context.Products.Any(a => a.Name == product.Name);
                if (check)
                {
                    ModelState.AddModelError("Name", "This product name already exists!");
                    ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name", product.AuthorID);
                    ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w => w.MainCatID != null && w.Status), "ID", "Name", product.CategoryID);
                    ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name", product.CoverID);
                    ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name", product.PublisherID);
                    return View(product);
                }

                if (ModelState.IsValid)
                {
                    string imagePath = string.Empty;

                    if (product.Image != null && product.Image.Length > 0)
                    {
                        // Set upload folder
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Product");
                        if (!Directory.Exists(uploadFolder))
                            Directory.CreateDirectory(uploadFolder);

                        // Unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        // Save file to wwwroot/images/product
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            product.Image.CopyTo(stream);
                        }

                        imagePath = "/Image/product/" + fileName; // Relative path for <img src="">
                    }

                    Product product2 = new Product
                    {
                        Name = product.Name,
                        Image = imagePath,
                        CategoryID = product.CategoryID,
                        AuthorID = product.AuthorID,
                        PublisherID = product.PublisherID,
                        CoverID = product.CoverID,
                        Price = product.Price,
                        Discount = product.Discount,
                        PageQuantity = product.PageQuantity,
                        Topic = product.Topic,
                        Description = product.Description,
                        Featured = product.Featured,
                        New = product.New,
                        PreOrderable = product.PreOrderable,
                        InStock = product.InStock,
                        BestSeller = product.BestSeller,
                        SlideShow = product.SlideShow,
                        Status = product.Status                      
                    };

                    _context.Add(product2);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name", product.AuthorID);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w => w.MainCatID != null && w.Status), "ID", "Name", product.CategoryID);
            ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name", product.CoverID);
            ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name", product.PublisherID);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name", product.AuthorID);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w => w.MainCatID != null && w.Status), "ID", "Name", product.CategoryID);
            ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name", product.CoverID);
            ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name", product.PublisherID);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,CategoryID,AuthorID,PublisherID,CoverID,Price,Discount,PageQuantity,Topic,Description,Featured,New,PreOrderable,InStock,BestSeller,SlideShow,Status")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            bool check = _context.Products.Any(a => a.Name == product.Name && a.ID != product.ID);
            if (check)
            {
                ModelState.AddModelError("Name", "This product name already exists!");

                ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name", product.AuthorID);
                ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w => w.MainCatID != null && w.Status), "ID", "Name", product.CategoryID);
                ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name", product.CoverID);
                ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name", product.PublisherID);
                return View(product);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    string? image = await _context.Products.Where(w => w.ID == id).Select(s => s.Image).FirstOrDefaultAsync();
                    if (image != null)
                    {
                        product.Image = image;

                        _context.Update(product);
                        int save = await _context.SaveChangesAsync();

                        if(save>0)
                        {
                            TempData["Success"] = "Product updated successfully!";
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
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
            ViewData["AuthorID"] = new SelectList(_context.Authors.Where(w => w.Status), "ID", "Name", product.AuthorID);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(w => w.MainCatID != null && w.Status), "ID", "Name", product.CategoryID);
            ViewData["CoverID"] = new SelectList(_context.Covers.Where(w => w.Status), "ID", "Name", product.CoverID);
            ViewData["PublisherID"] = new SelectList(_context.Publishers.Where(w => w.Status), "ID", "Name", product.PublisherID);
            return View(product);
        }

        public async Task<IActionResult> EditImage(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("Index", "Home");

            Product? product = await _context.Products.FindAsync(id);

            TempData["ProductID"] = id;

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(int id, IFormFile image)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("Index", "Home");

            try
            {
                int ID = Convert.ToInt32(TempData["ProductID"]);

                if (ID != id)
                {
                    TempData["Error"] = "Product Tracking Error, Unable to update Image!!!";
                    return RedirectToAction("Index");
                }

                Product? product = await _context.Products.FindAsync(id);

                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        string oldPartialPath = product.Image ?? string.Empty;
                        string oldFileName = oldPartialPath.Split('/').Last();
                        string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Product/") + oldFileName;

                        if (image != null && image.Length > 0)
                        {
                            if (System.IO.File.Exists(oldFullPath))
                            {
                                System.IO.File.Delete(oldFullPath);
                            }

                            // Unique file name
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            string FullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Product/") + fileName;

                            using (var stream = new FileStream(FullPath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }
                            string imagePath = "/Image/Product/" + fileName;
                            product.Image = imagePath;

                            _context.Update(product);
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
                            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Product");
                            if (!Directory.Exists(uploadFolder))
                                Directory.CreateDirectory(uploadFolder);

                            // Unique file name
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadFolder, fileName);

                            // Save file to wwwroot/images/product
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }

                            imagePath = "/Image/Product/" + fileName; // Relative path for <img src="">
                            product.Image = imagePath;
                            _context.Update(product);

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
                TempData["Error"] = "Product not found!!!";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Cover)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                string oldPartialPath = product.Image ?? string.Empty;
                string oldFileName = oldPartialPath.Split('/').Last();
                string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Product/") + oldFileName;
                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Delete(oldFullPath);
                }

                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
