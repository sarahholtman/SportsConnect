namespace SportsConnect.Models
{
    public class MyApplicationViewModel
    {
        public int ApplicationId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string Sport { get; set; } = string.Empty;

        public DateTime ApplicationDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}