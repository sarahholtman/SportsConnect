using System.ComponentModel.DataAnnotations;

namespace SportsConnect.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Pending";

        public string Responses { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int TeamId { get; set; }
    }
}