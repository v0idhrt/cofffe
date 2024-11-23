using System.ComponentModel.DataAnnotations;

namespace SupportSystem.Shared
{
    public class RegistrationRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
