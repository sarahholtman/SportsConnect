namespace SportsConnect.Models
{
    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string TeamName { get; set; } = string.Empty;

        public DateTime ApplicationDate { get; set; }

        public string Status { get; set; } = string.Empty;

        // Stores full application responses as a single text block for this iteration
        public string Responses { get; set; } = string.Empty;
    }
}