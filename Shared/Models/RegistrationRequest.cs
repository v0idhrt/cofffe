using System.ComponentModel.DataAnnotations;

namespace SupportSystemCofe.Shared.Models
{
    public class RegistrationRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string BusinessName { get; set; }
        public string Industry { get; set; }
        public string Region { get; set; }
        public string Scale { get; set; }
        public string Details { get; set; } // Поле для дополнительных деталей
    }
}
