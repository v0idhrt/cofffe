using Microsoft.AspNetCore.Mvc;
using SupportSystemCofe.Shared.Models;

namespace SupportSystemCofe.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Некорректные данные.");

            return Ok(new { Message = "Регистрация успешна!" });
        }
    }
}
