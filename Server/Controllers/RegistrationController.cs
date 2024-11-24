using Microsoft.AspNetCore.Mvc;
using Server.Controllers;
using SupportSystemCofe.Shared.Models;
using System.Net.Mail;
using Server;
using System.Security.Cryptography;
using Shared.Models;

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

            ControllerGlobals.dbControl = new DatabaseController();
            if (ControllerGlobals.dbControl.user_writeRegInfo(request) == 0)
            {
                return BadRequest("Такой пользователь уже существует!.");
            }

            MailAddress emailFrom = new MailAddress(Config.emailAddr, "Web Registration");
            MailAddress emailTo = new MailAddress(request.Email, "Web Registration");
            MailMessage confirmMessage = new MailMessage(emailFrom, emailTo);

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            
            byte[] inpBytes = System.Text.Encoding.UTF8.GetBytes(request.Email);
            string md5Hash = Convert.ToHexString(md5.ComputeHash(inpBytes));
            confirmMessage.Subject = "Email confirmation";
            confirmMessage.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                            "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>", 
                            "https://localhost:7120/api/confirmation/?value=" + md5Hash);
            confirmMessage.IsBodyHtml = true;
            

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential(Config.emailAddr, Config.emailPass);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Send(confirmMessage);
            ControllerGlobals.confirmInProcess.Add(md5Hash, request.Email);
            return Ok(new { Message = "Регистрация успешна!" });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        [HttpGet]
        public RegistrationRequest GetProfile([FromBody] EmailRequest request)
        {
            RegistrationRequest regInfo = new RegistrationRequest();

            regInfo = ControllerGlobals.dbControl.user_getRegInfo(request.Email);

            return regInfo;
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ConfirmationController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetProfile([FromBody] ConfirmationRequest request)
        {
            if (!ControllerGlobals.confirmInProcess.ContainsKey(request.Value))
            {
                return BadRequest("Такого запроса на подтверждение нет!.");
            }

            if (ControllerGlobals.dbControl.user_isActivated(ControllerGlobals.confirmInProcess[request.Value]))
                return BadRequest("Аккаунт уже подтвержден!.");

            if (ControllerGlobals.dbControl.user_activateAccount(ControllerGlobals.confirmInProcess[request.Value]) == false)
                return BadRequest("Аккаунт не подтвержден.");

            return Ok(new { Message = "Почта подтверждена!" });
        }
    }
}
