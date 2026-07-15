using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SportsConnect.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Province { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        // Password stored as plain text for this iteration (to be replaced with secure hashing)
        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }
}