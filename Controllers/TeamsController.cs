using Microsoft.AspNetCore.Mvc;
using SportsConnect.Data;
using SportsConnect.Models;
using System.Linq;

namespace SportsConnect.Controllers
{

    // Handles team browsing, creation, and leader management
    public class TeamsController : Controller
    {
        private readonly SportsConnectContext _context;

        public TeamsController(SportsConnectContext context)
        {
            _context = context;
        }

        // Displays all teams with filtering options based on user input
        public IActionResult Index(string keyword, string city, string sport, string experienceLevel)
        {
            var teams = from t in _context.Teams
                        select t;

            // prevent user from applying when their already in team or is team leader
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                teams = teams.Where(t =>
                    t.LeaderId != userId.Value &&
                    !_context.Memberships.Any(m =>
                        m.TeamId == t.TeamId &&
                        m.UserId == userId.Value &&
                        m.Status == "Active"));
            }

            // Applies keyword search across multiple fields
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();

                teams = teams.Where(t =>
                    t.TeamName.ToLower().Contains(keyword) ||
                    t.Location.ToLower().Contains(keyword) ||
                    t.Sport.ToLower().Contains(keyword) ||
                    t.ExperienceLevel.ToLower().Contains(keyword));
            }


            // Applies individual filters
            if (!string.IsNullOrEmpty(city))
            {
                city = city.ToLower();

                teams = teams.Where(t =>
                    t.Location.ToLower().Contains(city));
            }

            if (!string.IsNullOrEmpty(sport))
            {
                sport = sport.ToLower();

                teams = teams.Where(t =>
                    t.Sport.ToLower().Contains(sport));
            }

            if (!string.IsNullOrEmpty(experienceLevel))
            {
                experienceLevel = experienceLevel.ToLower();

                teams = teams.Where(t =>
                    t.ExperienceLevel.ToLower().Contains(experienceLevel));
            }

            return View(teams.ToList());
        }

        // Displays details for a specific team
        public IActionResult Details(int id)
        {
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // Opens the create team form
        public IActionResult Create()
        {
            return View();
        }

        // Creates a new team and assigns the logged-in user as the leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Team team)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            team.LeaderId = userId.Value;
            team.Location = $"{team.City}, {team.Province}, {team.Country}";
            ModelState.Remove("Location");

            if (ModelState.IsValid)
            {
                _context.Teams.Add(team);
                _context.SaveChanges();

                var leaderMembership = new Membership
                {
                    UserId = userId.Value,
                    TeamId = team.TeamId,
                    JoinDate = DateTime.Now,
                    Status = "Active",
                    Position = "Player"
                };

                _context.Memberships.Add(leaderMembership);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

                return View(team);
        }

        // Displays teams led by the current user
        public IActionResult Manage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var teams = _context.Teams
                .Where(t => t.LeaderId == userId.Value)
                .ToList();

            return View(teams);
        }

        // Opens edit form for a team (leader-only access)
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == id);

            // Makes sure only the team leader can edit
            if (team == null || team.LeaderId != userId.Value)
            {
                return RedirectToAction("Manage");
            }

            return View(team);
        }

        // Updates team details after edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Team updatedTeam)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == updatedTeam.TeamId);

            // Makes sure only the team leader can update
            if (team == null || team.LeaderId != userId.Value)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                team.TeamName = updatedTeam.TeamName;
                team.Sport = updatedTeam.Sport;
                // Rebuilds location string from individual fields
                team.Location = $"{team.City}, {team.Province}, {team.Country}";
                team.ExperienceLevel = updatedTeam.ExperienceLevel;

                _context.SaveChanges();

                return RedirectToAction("Manage");
            }

            return View(updatedTeam);
        }

        // Displays delete confirmation for a team
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == id);

            // Makes sure only the team leader can delete
            if (team == null || team.LeaderId != userId.Value)
            {
                return RedirectToAction("Manage");
            }

            return View(team);
        }

        // Deletes a team after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == id);

            if (team == null || team.LeaderId != userId.Value)
            {
                return RedirectToAction("Manage");
            }

            var applications = _context.Applications
                .Where(a => a.TeamId == team.TeamId)
                .ToList();

            _context.Applications.RemoveRange(applications);

            _context.Teams.Remove(team);
            _context.SaveChanges();

            return RedirectToAction("Manage");
        }
    }
}