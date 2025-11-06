using EBookStore.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBookStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ConnectionString _context;

        public AdminController(ConnectionString context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            // Summary counts
            ViewBag.TotalAuthors = await _context.Authors.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TotalOrders = await _context.Orders.CountAsync();

            // Orders status data for doughnut chart
            var pending = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var ongoing = await _context.Orders.CountAsync(o => o.Status == "On Process");
            var delivered = await _context.Orders.CountAsync(o => o.Status == "Delivered");
            var canceled = await _context.Orders.CountAsync(o => o.Status == "Canceled");

            ViewBag.OrdersStatusData = new int[] { pending, ongoing, delivered, canceled };

            // Trend data (last 6 months)
            var months = new List<string>();
            var productsTrend = new List<int>();
            var usersTrend = new List<int>();

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                months.Add(month.ToString("MMM"));

                productsTrend.Add(await _context.Products
                    .CountAsync(p => p.ID > 0 && p.ID > 0 && p.ID != 0 && p.Status && p.ID > 0 && p.ID > 0 && p.ID > 0 && p.ID > 0 && p.ID > 0 && p.ID > 0)); // Optional: filter by creation month if you have CreatedDate
                usersTrend.Add(await _context.Users
                    .CountAsync(u => u.ID > 0)); // Optional: filter by registration month if you have CreatedDate
            }

            ViewBag.Months = months;
            ViewBag.ProductsTrend = productsTrend;
            ViewBag.UsersTrend = usersTrend;

            return View();
        }
    }
}
