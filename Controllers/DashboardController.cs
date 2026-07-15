using Microsoft.AspNetCore.Mvc;
using SportsConnect.Data;
using SportsConnect.Models;

namespace SportsConnect.Controllers
{
    // Builds the main ddashboard for the logged-in user
    public class DashboardController : Controller
    {
        private readonly SportsConnectContext _context;

        public DashboardController(SportsConnectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Gets the current user for location-based dashboard content
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Combines several dashboard sections into one view model for the page.
            var viewModel = new DashboardViewModel
            {
                // Gets the teams the user belongs to and displays the team name instead of TeamId.
                MembershipDetails = _context.Memberships
                    .Where(m => m.UserId == userId)
                    .Select(m => new
                    {
                        // Finds all teams created by the logged-in user.
                        TeamName = _context.Teams
                            .Where(t => t.TeamId == m.TeamId)
                            .Select(t => t.TeamName)
                            .FirstOrDefault() ?? "Unknown Team",
                        m.JoinDate,
                        m.Status
                    })
                    .AsEnumerable()
                    .Select(m => (m.TeamName, m.JoinDate, m.Status))
                    .ToList(),

                TeamsILead = _context.Teams
                    .Where(t => t.LeaderId == userId)
                    .ToList(),

                AllTeams = _context.Teams
                    .ToList(),

                // Shows nearby teams by comparing team location text to the user's saved location.
                NearbyTeams = _context.Teams
                    .Where(t =>
                        t.Country == currentUser.Country &&
                        t.LeaderId != userId.Value &&
                        !_context.Memberships.Any(m =>
                            m.TeamId == t.TeamId &&
                            m.UserId == userId.Value &&
                            m.Status == "Active"))
                    .OrderByDescending(t =>
                        t.City == currentUser.City &&
                        t.Province == currentUser.Province &&
                        t.Country == currentUser.Country)
                    .ThenByDescending(t =>
                        t.Province == currentUser.Province &&
                        t.Country == currentUser.Country)
                    .ThenByDescending(t =>
                        t.Country == currentUser.Country)
                    .ToList(),
            };

            return View(viewModel);
        }
    }
}