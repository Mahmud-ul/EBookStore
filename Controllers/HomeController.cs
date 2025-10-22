using EBookStore.Models;
using EBookStore.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Diagnostics;

namespace EBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConnectionString _context;

        public HomeController(ConnectionString context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> Products = await _context.Products.Where(w => w.Status).Include(i=>i.Author).Include(j=>j.Category).ToListAsync();

            //Slide Show
            List<Product> SlideShow = new List<Product>();

            //Featured
            List<Product> Featured = new List<Product>();

            //Best Seller
            Product BestSeller = new Product();

            //New
            List<Product> Popular = new List<Product>();

            //Discount
            List<Product> Discount = new List<Product>();

            foreach (Product i in Products)
            {
                if(i.SlideShow )
                {
                    SlideShow.Add(i); 
                }
                if (i.Featured)
                {
                    Featured.Add(i);
                }
                if (i.BestSeller)
                {
                    BestSeller = i;
                }
                if (i.Popular)
                {
                    Popular.Add(i);
                }
                if (i.Discount!=null && i.Discount>0)
                {
                    Discount.Add(i);
                }
            }
            ViewBag.SlideShow = SlideShow;
            ViewBag.Featured = Featured;
            ViewBag.BestSeller = BestSeller;
            ViewBag.Popular = Popular;
            ViewBag.Discount = Discount;

            IEnumerable<Category> category = await _context.Categories.Where(w=>w.Status && w.Parent!= null && w.Parent.Name == "Book").ToListAsync();
            ViewBag.Category = category;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
