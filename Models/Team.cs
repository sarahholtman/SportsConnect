using System.ComponentModel.DataAnnotations;

namespace SportsConnect.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        public string TeamName { get; set; } = string.Empty;

        [Required]
        public string Sport { get; set; } = string.Empty;

        // Combined location string used for simplified searching and display
        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Experience Level")]
        public string ExperienceLevel { get; set; } = string.Empty;

        public int LeaderId { get; set; }

        public string City { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
    }
}