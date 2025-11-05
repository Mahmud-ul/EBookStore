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
    public class CartController : Controller
    {
        private readonly ConnectionString _context;

        public CartController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var carts = _context.Carts.Include(c => c.Product).Include(c => c.User);
            return View(await carts.ToListAsync());
        }

        // GET: Cart/Details?userID=5&productID=10
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email");
            return View();
        }

        // POST: Cart/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,UserID,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", cart.ProductID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", cart.UserID);
            return View(cart);
        }

        // GET: Cart/Edit?userID=5&productID=10
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cart == null)
            {
                return NotFound();
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", cart.ProductID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", cart.UserID);
            return View(cart);
        }

        // POST: Cart/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,UserID,Quantity")] Cart cart)
        {
            if (id != cart.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.ID))
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
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", cart.ProductID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", cart.UserID);
            return View(cart);
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.ID == id);
        }

        public async Task<IActionResult> CartList()
        {
            var carts = _context.Carts.Where(w=>w.UserID == HttpContext.Session.GetInt32("UserID")).Include(c => c.Product).Include(c => c.User);
            return View(await carts.ToListAsync());
        }
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");

            if (userID == null || userID == 0)
            {
                // Return JSON response indicating login is required
                return Json(new { success = false, requiresLogin = true, message = "Please log in to add items to your cart." });
            }

            bool exist = _context.Carts.Where(w => w.UserID == userID && w.ProductID == id).Any();

            if (exist)
                return Json(new { success = false, message = "Already added to the cart!" });

            Cart cart = new Cart
            {
                ProductID = id,
                UserID = userID ?? 0,
                Quantity = 1
            };
            _context.Add(cart);
            int save = _context.SaveChanges();

            int totalCart = _context.Carts
                    .Where(w => w.UserID == userID)
                    .Include(s => s.Product)
                    .ToList()
                    .Sum(s => ((s.Product?.Price ?? 0) - (s.Product?.Discount ?? 0)) * s.Quantity);

            HttpContext.Session.SetInt32("Cart", totalCart);

            if (save > 0)
            {
                return Json(new { success = true, message = "Item added to cart!", totalCart });
            }
            else
                return Json(new { success = false, message = "Failed to save" });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.ID == id);
            if (cart != null)
            {
                cart.Quantity = quantity;
                _context.SaveChanges();
            }

            int? userID = HttpContext.Session.GetInt32("UserID");
            int totalCart = 0;
            if (userID != null)
            {
                totalCart = _context.Carts
                    .Where(w => w.UserID == userID)
                    .Include(s => s.Product) // ensure Product is loaded
                    .ToList() // switch to client-side evaluation
                    .Sum(s => ((s.Product?.Price ?? 0) - (s.Product?.Discount ?? 0)) * s.Quantity);

                HttpContext.Session.SetInt32("Cart", totalCart);
            }

            return Json(new { success = true, totalCart });
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.ID == id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }

            int? userID = HttpContext.Session.GetInt32("UserID");
            int totalCart = 0;
            if (userID != null)
            {
                totalCart = _context.Carts
                    .Where(w => w.UserID == userID)
                    .Include(s => s.Product) // ensure Product is loaded
                    .ToList() // switch to client-side evaluation
                    .Sum(s => ((s.Product?.Price ?? 0) - (s.Product?.Discount ?? 0)) * s.Quantity);

                HttpContext.Session.SetInt32("Cart", totalCart);
            }

            return Json(new { success = true, totalCart });
        }      
    }
}