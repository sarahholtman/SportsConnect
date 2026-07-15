using Microsoft.AspNetCore.Mvc;
using SportsConnect.Data;
using SportsConnect.Models;

namespace SportsConnect.Controllers
{
    // Handles the team applications
    public class ApplicationsController : Controller
    {
        private readonly SportsConnectContext _context;

        public ApplicationsController(SportsConnectContext context)
        {
            _context = context;
        }

        // Shows applications for teams led by the user who is logged in
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Retrieves only aspplications for teams thaat the current user leads
            // Nested query matches applications to teams where the logged-in user is the leader
            var applications = _context.Applications
                .Where(a => _context.Teams
                    .Any(t => t.TeamId == a.TeamId && t.LeaderId == userId))
                .Select(a => new ApplicationViewModel
                {
                    ApplicationId = a.ApplicationId,
                    UserName = _context.Users
                        .Where(u => u.UserId == a.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown User",
                    TeamName = _context.Teams
                        .Where(t => t.TeamId == a.TeamId)
                        .Select(t => t.TeamName)
                        .FirstOrDefault() ?? "Unknown Team",
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status,
                    Responses = a.Responses
                })
                .ToList();

            return View(applications);
        }

        // Opens an application form for the selected team
        public IActionResult Create(int teamId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }


            var team = _context.Teams.FirstOrDefault(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound();
            }

            if (team.LeaderId == userId.Value)
            {
                TempData["Message"] = "You cannot apply to join a team that you lead.";
                return RedirectToAction("Index", "Teams");
            }

            var existingApplication = _context.Applications.FirstOrDefault(a =>
                a.TeamId == teamId &&
                a.UserId == userId.Value &&
                a.Status == "Pending");

            if (existingApplication != null)
            {
                TempData["Message"] = "You already have a pending application for this team.";
                return RedirectToAction("Index", "Teams");
            }

            var application = new Application
            {
                TeamId = teamId
            };

            return View(application);
        }

        // Saves the submitted application and store the form responses together
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application application, IFormCollection form)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // prevent duplicate application submissions
            var existingApplication = _context.Applications.FirstOrDefault(a =>
                a.TeamId == application.TeamId &&
                a.UserId == userId.Value &&
                a.Status == "Pending");

            if (existingApplication != null)
            {
                ModelState.AddModelError("", "You already have a pending application for this team.");
                return View(application);
            }
            if (ModelState.IsValid)
            {
                application.UserId = userId.Value;

                // Combines the application form fields into one response field.
                // Approach is to simplify storage, avoiding database tables.
                application.Responses =
                    $"Preferred Position: {form["PreferredPosition"]}\n" +
                    $"Reason for Joining: {form["ReasonForJoining"]}\n\n" +
                    $"Availability:\n" +
                    $"Monday: {form["Monday"]}\n" +
                    $"Tuesday: {form["Tuesday"]}\n" +
                    $"Wednesday: {form["Wednesday"]}\n" +
                    $"Thursday: {form["Thursday"]}\n" +
                    $"Friday: {form["Friday"]}\n" +
                    $"Saturday: {form["Saturday"]}\n" +
                    $"Sunday: {form["Sunday"]}\n\n" +
                    $"Skills/Achievements: {form["Skills"]}\n\n" +
                    $"Additional Comments: {form["Comments"]}";

                _context.Applications.Add(application);
                _context.SaveChanges();

                return RedirectToAction("Index", "Teams");
            }

            return View(application);
        }

        // Approves an application and creates an active membership for the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = "Approved";

            // When an application is approved, a membership record is created to link the user to the team.
            var membership = new Membership
            {
                UserId = application.UserId,
                TeamId = application.TeamId,
                JoinDate = DateTime.Now,
                Status = "Active"
            };

            _context.Memberships.Add(membership);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Denies an application (does not create a membership)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deny(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = "Denied";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Shows the full details of a submitted application
        public IActionResult Details(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == application.UserId);
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == application.TeamId);

            var currentUserId = HttpContext.Session.GetInt32("UserId");

            ViewBag.CanReview = currentUserId != null &&
                                team != null &&
                                team.LeaderId == currentUserId.Value;

            var viewModel = new ApplicationViewModel
            {
                ApplicationId = application.ApplicationId,
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                TeamName = team != null ? team.TeamName : "Unknown",
                ApplicationDate = application.ApplicationDate,
                Status = application.Status,
                Responses = application.Responses
            };

            return View(viewModel);
        }

        // Shows applications submitted by the logged-in user
        public IActionResult MyApplications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var applications = _context.Applications
                .Where(a => a.UserId == userId.Value)
                .Where(a => _context.Teams.Any(t => t.TeamId == a.TeamId))
                .Select(a => new MyApplicationViewModel
                {
                    ApplicationId = a.ApplicationId,

                    TeamName = _context.Teams
                        .Where(t => t.TeamId == a.TeamId)
                        .Select(t => t.TeamName)
                        .FirstOrDefault() ?? "Unknown",

                    Sport = _context.Teams
                        .Where(t => t.TeamId == a.TeamId)
                        .Select(t => t.Sport)
                        .FirstOrDefault() ?? "",

                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status
                })
                .ToList();

            return View(applications);
        }
    }
}