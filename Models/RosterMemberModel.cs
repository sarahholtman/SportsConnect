namespace SportsConnect.Models
{
    public class RosterMemberModel
    {
        public int MembershipId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime JoinDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Position { get; set; } = "Player";

        public bool IsTeamLead { get; set; }
    }
}