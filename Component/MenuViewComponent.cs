using EBookStore.Models;
using EBookStore.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBookStore.Component
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ConnectionString _context;

        public MenuViewComponent(ConnectionString context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type)
        {
            if(type == "Category")
            {
                IEnumerable<Category> categories = await _context.Categories.Where(w => w.MainCatID != null && w.Status).ToListAsync();
                return View("Category", categories);
            }
            else if(type == "Author")
            {
                IEnumerable<Author> authors = await _context.Authors.Where(w => w.Status).ToListAsync();
                return View("Author", authors);
            }
            return View("Default");
        }
    }
}
