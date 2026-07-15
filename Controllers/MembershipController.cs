using Microsoft.AspNetCore.Mvc;
using SportsConnect.Data;
using SportsConnect.Models;

namespace SportsConnect.Controllers
{

    // Handles team membership management, including rosters, roles, and user memberships
    public class MembershipsController : Controller
    {
        private readonly SportsConnectContext _context;

        public MembershipsController(SportsConnectContext context)
        {
            _context = context;
        }

        // Displays the roster for a specific team (leader access only)
        public IActionResult Index(int teamId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Check if user is the leader of this team
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == teamId);

            if (team == null || team.LeaderId != userId)
            {
                return Unauthorized();
            }

            // Retrieves roster members and maps user details for display
            var memberships = _context.Memberships
                .Where(m => m.TeamId == teamId)
                .Select(m => new RosterMemberModel
                {
                    MembershipId = m.MembershipId,
                    UserName = _context.Users
                        .Where(u => u.UserId == m.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown User",
                    Position = m.Position,
                    IsTeamLead = team.LeaderId == m.UserId,
                    JoinDate = m.JoinDate,
                    Email = _context.Users
                        .Where(u => u.UserId == m.UserId)
                        .Select(u => u.Email)
                        .FirstOrDefault() ?? "",
                    Status = m.Status
                })
                .ToList();

            ViewBag.TeamName = team.TeamName;

            return View(memberships);
        }

        // Displays a summary of all teams the user leads, including member counts
        public IActionResult LeaderRosters()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var teams = _context.Teams
                .Where(t => t.LeaderId == userId.Value)
                .ToList();

            // Builds a summary view of each team and its members
            var rosterSummaries = teams.Select(team => new TeamRosterSummaryViewModel
            {
                TeamId = team.TeamId,
                TeamName = team.TeamName,
                Sport = team.Sport,
                Location = team.Location,
                ExperienceLevel = team.ExperienceLevel,

                Members = _context.Memberships
                    .Where(m => m.TeamId == team.TeamId)
                    .Select(m => new RosterMemberModel
                    {
                        MembershipId = m.MembershipId,
                        UserName = _context.Users
                            .Where(u => u.UserId == m.UserId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? "Unknown User",

                        Email = _context.Users
                            .Where(u => u.UserId == m.UserId)
                            .Select(u => u.Email)
                            .FirstOrDefault() ?? "",

                        Position = m.Position,
                        IsTeamLead = team.LeaderId == m.UserId,
                        JoinDate = m.JoinDate,
                        Status = m.Status
                    })
                    .ToList()
            }).ToList();

            // Calculates member count for each team summary.
            foreach (var roster in rosterSummaries)
            {
                roster.CurrentMembers = roster.Members.Count;
            }

            return View(rosterSummaries);
        }

        // Updates a member's position within a team (inline update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePosition(int membershipId, string position)
        {
            var membership = _context.Memberships.FirstOrDefault(m => m.MembershipId == membershipId);

            if (membership == null)
            {
                return NotFound();
            }

            membership.Position = string.IsNullOrEmpty(position) ? "" : position;
            _context.SaveChanges();

            return RedirectToAction("Index", new { teamId = membership.TeamId });
        }

        // Displays all teams the logged-in user is a member of
        public IActionResult MyMemberships()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Combines membership and team data into a view model
            var memberships = _context.Memberships
                .Where(m => m.UserId == userId.Value)
                .Select(m => new MembershipTeamViewModel
                {
                    MembershipId = m.MembershipId,

                    TeamName = _context.Teams
                        .Where(t => t.TeamId == m.TeamId)
                        .Select(t => t.TeamName)
                        .FirstOrDefault() ?? "Unknown Team",

                    Sport = _context.Teams
                        .Where(t => t.TeamId == m.TeamId)
                        .Select(t => t.Sport)
                        .FirstOrDefault() ?? "",

                    Location = _context.Teams
                        .Where(t => t.TeamId == m.TeamId)
                        .Select(t => t.Location)
                        .FirstOrDefault() ?? "",

                    ExperienceLevel = _context.Teams
                        .Where(t => t.TeamId == m.TeamId)
                        .Select(t => t.ExperienceLevel)
                        .FirstOrDefault() ?? "",

                    Position = string.IsNullOrEmpty(m.Position) ? "N/A" : m.Position,

                    JoinDate = m.JoinDate,
                    Status = m.Status,

                    // Retrieves team leader email for communication
                    LeaderEmail = _context.Users
                        .Where(u => u.UserId == _context.Teams
                            .Where(t => t.TeamId == m.TeamId)
                            .Select(t => t.LeaderId)
                            .FirstOrDefault())
                        .Select(u => u.Email)
                        .FirstOrDefault() ?? ""
                })
                .ToList();

            return View(memberships);
        }

        // Allows a user to leave a team by removing their membership record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeaveTeam(int membershipId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var membership = _context.Memberships.FirstOrDefault(m =>
                m.MembershipId == membershipId &&
                m.UserId == userId.Value);

            if (membership == null)
            {
                return RedirectToAction("MyMemberships");
            }

            _context.Memberships.Remove(membership);
            _context.SaveChanges();

            return RedirectToAction("MyMemberships");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveMember(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var membership = _context.Memberships.FirstOrDefault(m => m.MembershipId == id);

            if (membership == null)
            {
                return NotFound();
            }

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == membership.TeamId);

            if (team == null || team.LeaderId != userId.Value)
            {
                return Unauthorized();
            }

            _context.Memberships.Remove(membership);
            _context.SaveChanges();

            return RedirectToAction("Index", new { teamId = team.TeamId });
        }
    }
}