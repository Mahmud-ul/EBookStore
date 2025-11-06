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
    public class OrderProductController : Controller
    {
        private readonly ConnectionString _context;

        public OrderProductController(ConnectionString context)
        {
            _context = context;
        }

        // GET: OrderProduct
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var connectionString = _context.OrderProducts.Include(o => o.Order).Include(o => o.Product);
            return View(await connectionString.ToListAsync());
        }

        // GET: OrderProduct/Details/5
        public async Task<IActionResult> Details(int? orderID, int? productID)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (orderID == null || productID == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderID == orderID && m.ProductID == productID);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return View(orderProduct);
        }

        // GET: OrderProduct/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            ViewData["OrderID"] = new SelectList(_context.Orders, "ID", "ID");
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name");
            return View();
        }

        // POST: OrderProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,ProductID,Quantity,Price")] OrderProduct orderProduct)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(orderProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "ID", "ID", orderProduct.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // GET: OrderProduct/Edit/5
        public async Task<IActionResult> Edit(int? orderID, int? productID)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (orderID == null || productID == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts.Where(w=>w.ProductID == productID && w.OrderID == orderID).FirstOrDefaultAsync();
            if (orderProduct == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "ID", "ID", orderProduct.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // POST: OrderProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,ProductID,Quantity,Price")] OrderProduct orderProduct)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id != orderProduct.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderProductExists(orderProduct.OrderID))
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
            ViewData["OrderID"] = new SelectList(_context.Orders, "ID", "ID", orderProduct.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // GET: OrderProduct/Delete/5
        public async Task<IActionResult> Delete(int? orderID, int? productID)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (orderID == null || productID == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderID == orderID && m.ProductID == productID);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return View(orderProduct);
        }

        public async Task<IActionResult> DeleteConfirmed(int? orderID, int? productID)
        {
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var orderProduct = await _context.OrderProducts.Where(w => w.ProductID == productID && w.OrderID == orderID).FirstOrDefaultAsync();
            if (orderProduct != null)
            {
                _context.OrderProducts.Remove(orderProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderProductExists(int id)
        {
            return _context.OrderProducts.Any(e => e.OrderID == id);
        }
    }
}
