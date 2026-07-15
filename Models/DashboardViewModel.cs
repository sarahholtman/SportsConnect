namespace SportsConnect.Models
{
    public class DashboardViewModel
    {
        public List<Membership> MyMemberships { get; set; } = new List<Membership>();

        public List<Team> TeamsILead { get; set; } = new List<Team>();

        public List<Team> AllTeams { get; set; } = new List<Team>();

        public List<Team> NearbyTeams { get; set; } = new List<Team>();

        public List<(string TeamName, DateTime JoinDate, string Status)> MembershipDetails { get; set; }
            = new List<(string, DateTime, string)>();
    }
}