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
    public class UserController : Controller
    {
        private readonly ConnectionString _context;

        public UserController(ConnectionString context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var connectionString = _context.Users.Include(u => u.UserType);
            return View(await connectionString.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailAvailable(string email, int id)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == email && u.ID != id);
            return Json(!emailExists); // Important: false means "not valid"
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsPhoneAvailable(string phone, int id)
        {
            bool phoneExists = await _context.Users.AnyAsync(u => u.Phone == phone && u.ID != id);
            return Json(!phoneExists); // Important: false means "not valid"
        }

        // GET: User/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            ViewData["UserTypeID"] = new SelectList(_context.UserTypes, "ID", "Name");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Email,Phone,Password,Status,UserTypeID")] User user)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserTypeID"] = new SelectList(_context.UserTypes, "ID", "Name", user.UserTypeID);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["UserTypeID"] = new SelectList(_context.UserTypes, "ID", "Name", user.UserTypeID);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Email,Phone,Status,UserTypeID,Password")] User user)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? password = await _context.Users.Where(w => w.ID == user.ID).Select(s => s.Password).FirstOrDefaultAsync();
                    if(password != null)
                    {
                        user.Password = password;
                        _context.Update(user);
                        int save = await _context.SaveChangesAsync();

                        if(save>0)
                            TempData["Success"] = "User updated successfully!";
                    }
                    else
                        TempData["Error"] = "Unable to update User, Please try again later!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            ViewData["UserTypeID"] = new SelectList(_context.UserTypes, "ID", "Name", user.UserTypeID);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            bool check = _context.Users.Any(e => e.ID == id);

            if (check)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                TempData["Error"] = "User not found!";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int id, string password, string confirmPassword)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (password == confirmPassword)
            {
                User? user = _context.Users.FirstOrDefault(e => e.ID == id);
                if(user != null)
                {
                    user.Password = password;
                    _context.Update(user);
                    int save = _context.SaveChanges();
                    if (save > 0)
                    {
                        TempData["Success"] = "Password reset successful!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Error"] = "User not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Password and Confirm Password aren't matching!";
            }
            ViewBag.id = id;
            return View();
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
