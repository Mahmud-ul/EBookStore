using EBookStore.Models;
using EBookStore.Models.CreateModel;
using EBookStore.Models.Database;
using EBookStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace EBookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ConnectionString _db;
        private readonly EmailService _emailService;
        private readonly IMemoryCache _memory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ConnectionString db, EmailService emailService, IMemoryCache memory, ILogger<AccountController> logger)
        {
            _db = db;
            _emailService = emailService;
            _memory = memory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginUserModel user)
        {
            try
            {
                // ⚠️ TODO: Hash passwords in production!
                User? matchedUser = _db.Users
                    .Include(u => u.UserType)
                    .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

                if (matchedUser != null)
                {
                    // Set session data
                    HttpContext.Session.SetString("UserName", matchedUser.Name.Split(' ')[0]);
                    HttpContext.Session.SetInt32("UserID", matchedUser.ID);
                    HttpContext.Session.SetString("UserType", matchedUser.UserType?.Name ?? "");
                    HttpContext.Session.SetInt32("UserTypeID", matchedUser.UserTypeID);

                    #region Cache Memory value set   
                    string cacheKey = matchedUser.UserType?.Name ?? "";
                    if (!string.IsNullOrEmpty(cacheKey))
                    {
                        // Get permissions (this both checks AND gets the value)
                        if (!_memory.TryGetValue(cacheKey, out List<string>? permissions))
                        {
                            permissions = _db.RolePermissions
                                .Where(w => w.RoleID == matchedUser.UserTypeID)
                                .Include(i => i.ActionRoute)
                                .Where(i => i.ActionRoute != null)
                                .Select(s => $"{s.ActionRoute!.Controller} - {s.ActionRoute.Action}")
                                .ToList();

                            _memory.Set(cacheKey, permissions, TimeSpan.FromHours(1));

                            if (permissions.Count == 0)
                            {
                                _logger.LogWarning($"Role {cacheKey} has ZERO permissions assigned!");
                            }
                        }
                    }
                    #endregion

                    // Calculate cart total (server-side)
                    int cart = _db.Carts
                        .Where(w => w.UserID == matchedUser.ID)
                        .Sum(s => ((s.Product != null ? s.Product.Price : 0) -
                                   (s.Product != null ? s.Product.Discount : 0)) * s.Quantity)??0;

                    HttpContext.Session.SetInt32("Cart", cart);

                    _logger.LogInformation($"User {matchedUser.Email} logged in successfully");
                    return RedirectToAction("Index", "Home");
                }

                // ⚠️ CONSIDER: Remove hardcoded admin or move to config
                // For development only - remove in production!
                else if (user.Email == "xonos" && user.Password == "Tipu152338")
                {
                    HttpContext.Session.SetString("UserName", "xonos");
                    HttpContext.Session.SetInt32("UserID", 0);
                    HttpContext.Session.SetString("UserType", "SuperAdmin");

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Message = "Invalid Email or password!";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewBag.Message = "An error occurred during login. Please try again.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optional: Clear specific cache entries for this user
            // But don't clear role caches as other users need them

            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

<<<<<<< HEAD
        // Rest of your methods (ForgotPassword, ResetPassword, etc.) are fine
        // ... keep them as is ...
=======
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user)
        {
            user.UserTypeID = _db.UserTypes.Where(w => w.Name == "User").Select(s => s.ID).FirstOrDefault();
            user.Status = true;
            if (user.UserTypeID > 0)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    TempData["Error"] = "Password and Confirm Password aren't Matching!!!";
                    return View();
                }

                _db.Users.Add(user);
                int save = _db.SaveChanges();

                if (save > 0)
                {
                    TempData["Success"] = "User Signup Successful. Please login!";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = "Failed to Save Information!!!";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                {
                    ViewBag.Error = "This email is not valid!!!";
                    return View();
                }
                bool existEmail = _db.Users.Any(a => a.Email == email);
                if (!existEmail)
                {
                    ViewBag.Error = "This email is not registered with us. Please check and try again!!!";
                    return View();
                }

                string code = CodeGenerator();
                List<string> emailAddress = new List<string> { email };
                string emailSubject = "Password reset Code";
                string emailBody = "Your Password reset Code is: " + code;

                bool sent = await SendMail(emailAddress, emailSubject, emailBody);
                if (sent)
                {
                    TempData.Clear();
                    TempData["Code"] = code;
                    TempData["Email"] = email;
                    return RedirectToAction("ResetPassword");
                }
                TempData["Error"] = "Code verification Failed. Please try again after some time!!!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            TempData["Code"] = TempData["Code"];
            TempData["Email"] = TempData["Email"];
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string code, string password, string confirmPassword)
        {
            string? email = (string?)TempData["Email"];
            if ((string?)TempData["Code"] == code && password == confirmPassword)
            {
                //Reset Password                                
                if (email != null)
                {
                    User? user = _db.Users.Where(w => w.Email == email).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = password;

                        _db.Users.Update(user);
                        int save = _db.SaveChanges();

                        if (save > 0)
                        {
                            TempData["Success"] = "Password Reset Successful!!!";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            else
            {
                ViewBag.Error = "Wrong Code or Password and ConfirmPassword aren't Matching!!!";
            }
            TempData["Email"] = email;
            TempData["Code"] = TempData["Code"];
            return View();
        }
>>>>>>> 199720b95032cbd73f24c22ad0fcacea95219641

        public String CodeGenerator()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] result = new char[6];

            for (int i = 0; i < 6; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }

        public async Task<bool> SendMail(List<string> emailAddress, string emailSubject, string emailBody)
        {
            return await _emailService.SendEmailAsync(emailAddress, emailSubject, emailBody);
        }
    }
}
