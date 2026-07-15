namespace SportsConnect.Models
{
    public class TeamRosterSummaryViewModel
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string Sport { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string ExperienceLevel { get; set; } = string.Empty;

        public int CurrentMembers { get; set; }

        public List<RosterMemberModel> Members { get; set; } = new List<RosterMemberModel>();
    }
}