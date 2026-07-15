namespace SportsConnect.Models
{

    // Used to display team membership details on the My Memberships page
    public class MembershipTeamViewModel
    {
        public int MembershipId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string Sport { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string ExperienceLevel { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public DateTime JoinDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string LeaderEmail { get; set; } = string.Empty;
    }
}