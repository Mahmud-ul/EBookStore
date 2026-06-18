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
    public class OrderController : Controller
    {
        private readonly ConnectionString _context;

        public OrderController(ConnectionString context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var connectionString = _context.Orders.Include(o => o.User);
            return View(await connectionString.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderDate,StatusDate,Status,UserID,TotalAmount,Name,Phone,City,Area,Address,PaymentMethod,DeliveryCharge")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", order.UserID);
            return View(order);
        }

        // GET: Order/Edit/5
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
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", order.UserID);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrderDate,StatusDate,Status,UserID,TotalAmount,Name,Phone,City,Area,Address,PaymentMethod,DeliveryCharge")] Order order)
        {
            if (id != order.ID)
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
                    if (!OrderExists(order.ID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "Email", order.UserID);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            // Your order creation logic here

            //Show the details including delivery charge and a payment button.
            //on the payment, pay the amount via bikash, nagad etc. 
            //If the payment success, Order placed and confirmed.
            //cart list cleared.

            int? userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
            {
                TempData["Error"] = "Please Login to Place order!";

                return RedirectToAction("Index", "Home");
            }

            IEnumerable<Cart> cart = _context.Carts.Where(w => w.UserID == userID).Include(i => i.Product).ToList();

            return View(cart);
        }

        [HttpPost]
        public IActionResult ConfirmOrder(string name, string phone, string city, string area, string address, string PaymentMethod, int DeliveryCharge, int total)
        {
            try
            {
                //Complete the payment here

                //Create Order
                int? userID = HttpContext.Session.GetInt32("UserID");

                if (userID == null)
                {
                    TempData["Error"] = "Please Login to Place order!";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Order order = new Order
                    {
                        OrderDate = DateTime.Now,
                        StatusDate = DateTime.Now,
                        Status = "On Process",
                        UserID = userID ?? 0,
                        Name = name,
                        Phone = phone,
                        City = city,
                        Area = area,
                        Address = address,
                        PaymentMethod = PaymentMethod,
                        TotalAmount = total,
                        DeliveryCharge = DeliveryCharge
                    };
                    _context.Add(order);

                    int save = _context.SaveChanges();
                    if (save > 0)
                    {
                        int orderID = _context.Orders.Where(w => w.UserID == userID && w.OrderDate == order.OrderDate).Select(s => s.ID).FirstOrDefault();
                        if (orderID > 0)
                        {
                            List<OrderProduct> orderProducts = new List<OrderProduct>();
                            IEnumerable<Cart> cart = _context.Carts.Where(w => w.UserID == userID).Include(i => i.Product).ToList();

                            foreach (Cart c in cart)
                            {
                                OrderProduct orp = new OrderProduct();
                                orp.OrderID = orderID;
                                orp.ProductID = c.ProductID;
                                orp.Price = c.Product != null ? (c.Product.Price) - (c.Product.Discount ?? 0) : 0;
                                orp.Quantity = c.Quantity;
                                orderProducts.Add(orp);
                            }

                            _context.AddRange(orderProducts);
                            _context.RemoveRange(cart);
                            
                            int save2 = _context.SaveChanges();
                            if (save2 > 0)
                            {
                                TempData["Success"] = "Order placed successfully. Thank you for shopping with us!";
                                HttpContext.Session.SetInt32("Cart", 0);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong, Please try again later!\n Error: " + ex.Message;
            }
            TempData["Error"] = "Order Confirmation failed!";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> OrderList()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");

            if (userID == null)
            {
                TempData["Error"] = "Please Login to view your order!";

                return RedirectToAction("Index", "Home");
            }

            IEnumerable<Order> orders = await _context.Orders.Where(w=> w.UserID == userID).ToListAsync(); 
            
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            Order? order = _context.Orders.Where(w => w.ID == id).Include(i => i.OrderProducts!).ThenInclude(j=>j.Product).FirstOrDefault();

            if(order == null)
            {
                TempData["Error"] = "Something went wrong. Please try again later.";
                return RedirectToAction("OrderList");
            }

            return View(order);
        }
    }
}
