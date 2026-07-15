using System.ComponentModel.DataAnnotations;

namespace SportsConnect.Models
{
    public class Membership
    {
        public int MembershipId { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Active";

        public int UserId { get; set; }

        public int TeamId { get; set; }

        // Optional role within the team
        public string Position { get; set; } = "Player";
    }
}