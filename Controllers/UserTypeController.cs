using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBookStore.Models;
using EBookStore.Models.Database;
using EBookStore.Models.Filters;

namespace EBookStore.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly ConnectionString _context;

        public UserTypeController(ConnectionString context)
        {
            _context = context;
        }

        // GET: UserType
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            return View(await _context.UserTypes.ToListAsync());
        }

        // GET: UserType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var userType = await _context.UserTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userType == null)
            {
                return NotFound();
            }

            return View(userType);
        }

        // GET: UserType/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: UserType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Status")] UserType userType)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            try
            {
                bool check = _context.UserTypes.Any(a => a.Name == userType.Name);
                if (check)
                {
                    ModelState.AddModelError("Name", "This User-Type title already exists!");
                    return View(userType);
                }
                else if (ModelState.IsValid)
                {
                    _context.Add(userType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View(userType);
        }

        // GET: UserType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var userType = await _context.UserTypes.FindAsync(id);
            if (userType == null)
            {
                return NotFound();
            }
            return View(userType);
        }

        // POST: UserType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Status")] UserType userType)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id != userType.ID)
            {
                return NotFound();
            }

            bool check = _context.UserTypes.Any(a => a.Name == userType.Name && a.ID != userType.ID);
            if (check)
            {
                ModelState.AddModelError("Name", "This User-Type title already exists!");
                return View(userType);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTypeExists(userType.ID))
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
            return View(userType);
        }

        // GET: UserType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin" && HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var userType = await _context.UserTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userType == null)
            {
                return NotFound();
            }

            return View(userType);
        }

        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "SuperAdmin")
                return RedirectToAction("Index", "Home");

            var userType = await _context.UserTypes.FindAsync(id);
            if (userType != null)
            {
                _context.UserTypes.Remove(userType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserTypeExists(int id)
        {
            return _context.UserTypes.Any(e => e.ID == id);
        }

        [HttpGet]
        public async Task<IActionResult> RolePermission(int id)
        {
            UserType? type = await _context.UserTypes.FindAsync(id);

            if(type != null)
            {
                IEnumerable<ActionRoute> actions = await _context.ActionRoutes.Where(w=>w.Status).ToListAsync();

                IEnumerable<RolePermission> permissions = await _context.RolePermissions.Where(w=>w.RoleID == id).ToListAsync();   
                
                AssignRolePermissionCreateModel assign = new AssignRolePermissionCreateModel
                {
                    Role = type,
                    RolePermissions = permissions,
                    ActionRoutes = actions
                };

                return View(assign);
            }

            TempData["Error"] = "User role not found!";

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RolePermission(int RoleId, List<int> PermissionIds)
        {
            try
            {
                // Remove existing permissions for this role
                var existingPermissions = _context.RolePermissions.Where(rp => rp.RoleID == RoleId);
                _context.RolePermissions.RemoveRange(existingPermissions);

                if (PermissionIds != null && PermissionIds.Any())
                {
                    // Add new permissions
                    foreach (var permissionId in PermissionIds)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleID = RoleId,
                            ActionRouteID = permissionId
                        };
                        _context.RolePermissions.Add(rolePermission);
                    }
                }

                int save = await _context.SaveChangesAsync();

                if (save > 0)
                    TempData["Success"] = "Permissions saved successfully";
                else
                    TempData["Error"] = "Permission assigning failed. Please try again later!";

                return RedirectToAction("Index", "UserType");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "UserType");
            }
        }
    }
}
