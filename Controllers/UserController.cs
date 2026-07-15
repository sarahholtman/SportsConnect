using Microsoft.AspNetCore.Mvc;
using SportsConnect.Data;
using SportsConnect.Models;
using Microsoft.AspNetCore.Identity;

namespace SportsConnect.Controllers
{
    // Handles user authentication and profile management
    public class UsersController : Controller
    {
        private readonly SportsConnectContext _context;

        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UsersController(SportsConnectContext context)

        {
            _context = context;
        }

        // Opens the registration page
        public IActionResult Register()
        {
            return View();
        }

        // Creates a new user account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            // prevents creation of duplicate accounts (same email address)
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());

            if (existingUser != null)

            // prevention of duplication accounts, no two accounts can have the same email addresses
            {
                ModelState.AddModelError("Email", "An account with this email already exists. Try logging in instead.");
                return View(user);
            }
            if (ModelState.IsValid)
            {
                user.Role = "User";

                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Opens the login page
        public IActionResult Login()
        {
            return View();
        }

        // Authenticates the user and starts a session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            // Authentication check
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.FirstName);

                    return RedirectToAction("Index", "Dashboard");
                }
            }
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // Clears session and logs the user out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Users");
        }

        // Displays the current user's profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Opens the edit profile page
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Updates user profile information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.City = updatedUser.City;
                user.Province = updatedUser.Province;
                user.Country = updatedUser.Country;

                _context.SaveChanges();

                return RedirectToAction("Profile");
            }

            return View(updatedUser);
        }
    }
}