using EBookStore.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace EBookStore.Models.Filters
{
    public class SimpleActionFilter : IActionFilter
    {
        private readonly ConnectionString _context;
        private readonly IMemoryCache _memory;  // ✅ Changed to IMemoryCache
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SimpleActionFilter(ConnectionString context, IMemoryCache memory, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _memory = memory;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Get controller and action names
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
            var action = context.RouteData.Values["action"]?.ToString() ?? "";
            var method = context.HttpContext.Request.Method;
            var controllerInstance = context.Controller as Controller;

            // Get session and role
            var session = _httpContextAccessor.HttpContext?.Session;
            string role = session?.GetString("UserType") ?? "";

            if(controller == "Home" || controller == "Account" || (controller == "Product" && (action == "ProductList" || action == "ProductDetails")) || (controller == "Cart" && (action == "CartList" || action == "AddToCart" || action == "Remove" || action == "UpdateQuantity")) || (controller == "Order" && (action == "OrderList" || action == "PlaceOrder")))
                return;
            else if (role == "SuperAdmin")
                return;
            else if (role == "Viewer")
            {
                if (method == "GET")
                    return;
                // Check if it's an AJAX request
                bool isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjax)
                {
                    // For AJAX, return JSON so client can handle redirect
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        redirect = "/Home/Index",
                        error = "This ID is for View Only"
                    });
                    return;
                }

                if (controllerInstance != null)
                {
                    // Now you can set TempData
                    controllerInstance.TempData["Error"] = "This ID is for View Only";
                }
                //TempData["Error"] = "This ID is for View Only";
                context.Result = new RedirectToActionResult("Index", "Home", null);
                Console.WriteLine("✓ Redirect set to Home/Index");
                return;
            }

            // If no role, user is not logged in - block access
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new UnauthorizedResult(); // or ForbidResult
                return;
            }

            #region Check Permission (Single Cache Lookup)

            // Try to get permissions from cache
            if (!_memory.TryGetValue(role, out List<string>? permissions))
            {
                // Cache MISS - Load from database
                permissions = _context.RolePermissions
                    .Where(w => w.UserType != null && w.UserType.Name == role)
                    .Include(i => i.ActionRoute)
                    .Where(i => i.ActionRoute != null)
                    .Select(s => $"{s.ActionRoute!.Controller.ToUpper()} - {s.ActionRoute.Action.ToUpper()}")
                    .ToList();

                // Store in cache with expiration
                _memory.Set(role, permissions, TimeSpan.FromHours(1));
            }

            // Now check if user has permission
            string requiredPermission = $"{controller} - {action}".ToUpper();

            if (permissions != null && permissions.Contains(requiredPermission))
            {
                // ✅ HAS PERMISSION - Continue normally
                Console.WriteLine($"✓ ALLOWED: {role} accessing {requiredPermission}");
                return;
            }

            // ❌ NO PERMISSION - Block
            Console.WriteLine($"✗ BLOCKED: {role} tried to access {requiredPermission}");
            context.Result = new ForbidResult();
            #endregion
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Leave empty or use for logging
        }
    }
}
