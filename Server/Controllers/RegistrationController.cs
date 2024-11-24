using Microsoft.AspNetCore.Mvc;
using Server.Controllers;
using SupportSystemCofe.Shared.Models;
using System.Net.Mail;
using Server;
using System.Security.Cryptography;
using Shared.Models;
using AIcontrolComputer.Models.AIAnswerProcessing;

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
        [HttpGet("{email}")]
        public IActionResult GetProfile(string email)
        {
            // Получаем данные пользователя из базы
            var regInfo = ControllerGlobals.dbControl.user_getRegInfo(email);

            if (regInfo == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Возвращаем данные пользователя
            return Ok(regInfo);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ConfirmationController : ControllerBase
    {
        [HttpGet]
        public IActionResult ConfirmAcc([FromQuery] string value)
        {
            if (!ControllerGlobals.confirmInProcess.ContainsKey(value))
            {
                return BadRequest("Такого запроса на подтверждение нет!");
            }

            if (ControllerGlobals.dbControl.user_isActivated(ControllerGlobals.confirmInProcess[value]))
            {
                return BadRequest("Аккаунт уже подтвержден!");
            }

            if (!ControllerGlobals.dbControl.user_activateAccount(ControllerGlobals.confirmInProcess[value]))
            {
                return BadRequest("Аккаунт не подтвержден.");
            }

            // Перенаправляем на абсолютный URL клиента
            return Redirect("https://localhost:7261/login");
        }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Получаем данные пользователя из базы
            var regInfo = ControllerGlobals.dbControl.user_getRegInfo(request.Email);

            if (regInfo == null)
            {
                return BadRequest("Пользователь с таким email не найден.");
            }

            if (regInfo.Password != request.Password)
            {
                return BadRequest("Неверный пароль.");
            }

            // Возвращаем данные профиля
            return Ok(new
            {
                Message = "Вход успешно выполнен!",
                ProfileUrl = $"/profile/{regInfo.Email}" 
            });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("registrations")]
        public IActionResult GetAllRegistrations()
        {
            var dbController = new DatabaseController();
            var users = dbController.GetAllUsers();

            if (users == null || users.Count == 0)
            {
                return NotFound("Нет данных для отображения.");
            }

            return Ok(users);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ConsultantNewsController _consultantNewsController;
        private readonly RgBusinessController _rgBusinessController;
        private readonly IResponseProcessingModule _responseProcessingModule;

        public NewsController(ConsultantNewsController consultantNewsController, IResponseProcessingModule responseProcessingModule, RgBusinessController rgBusinessController)
        {
            _consultantNewsController = consultantNewsController;
            _rgBusinessController = rgBusinessController;
            _responseProcessingModule = responseProcessingModule;
        }

        [HttpGet("important")]
        public async Task<IActionResult> GetImportantNews()
        {
            try
            {
                // Получаем все новости из парсера
                var newsInfoCons = _consultantNewsController.GetNewsInfo();
                var newsInfoRg = _rgBusinessController.GetNewsInfo();
                newsInfoCons.AddRange(newsInfoRg);


                var importantNews = new List<Grant>();

                foreach (var item in newsInfoCons)
                {
                    string prompt = $"Ты - бизнес консультант, задача которого проанализировать " +
                        $"юридическую новость и определить - важна ли она для бизнеса. " +
                        $"В случае, если новость важна бизнесу, укажи в начале ответа '+' и кратко выдели главную суть. " +
                        $"В случае, если новость не важна бизнесу, укажи в качестве ответа только '-'. " +
                        $"Ниже приведена новость:\n" +
                        $"{item.Item1}\n" +
                        $"{item.Item2}";

                    // Отправляем запрос к ИИ
                    var aiResult = await _responseProcessingModule.GetGptAnswer(prompt);

                    // Если ИИ ответил, что новость важна, добавляем её в список
                    if (!string.IsNullOrEmpty(aiResult) && aiResult[0] == '+')
                    {
                        importantNews.Add(new Grant
                        {
                            Title = item.Item1,
                            Description = item.Item2,
                            AIAnalysis = aiResult.Substring(1).Trim() // Убираем '+'
                        });
                    }
                }

                return Ok(importantNews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обработке новостей: {ex.Message}");
            }
        }
    }

    public class Grant
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AIAnalysis { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IResponseProcessingModule _responseProcessingModule;

        public AIController(IResponseProcessingModule responseProcessingModule)
        {
            _responseProcessingModule = responseProcessingModule;
        }

        [HttpPost("selected-responses")]
        public async Task<IActionResult> GetSelectedAIResponses([FromBody] AIRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserInput) || request.SelectedModels == null || request.SelectedModels.Count == 0)
            {
                return BadRequest("Некорректный запрос. Убедитесь, что указаны входные данные и выбранные модели.");
            }

            try
            {
                // Формируем промт для анализа бизнеса
                string prompt = $"Вы являетесь искусственным интеллектом, специализирующимся на анализе бизнеса. " +
                                $"Ваша задача — провести краткий анализ бизнеса на основе следующих данных: \n\n" +
                                $"Данные бизнеса:\n{request.UserInput}\n\n" +
                                $"Проанализируйте:\n" +
                                $"1. Перспективы успеха бизнеса в будущем.\n" +
                                $"2. Потенциал бизнеса в его отрасли.\n" +
                                $"3. В конце выведите свою оценку на основе 100-балльной шкалы (где 100 — это высокий потенциал для инвестиций, а 0 — не рекомендуется инвестировать).\n\n" +
                                $"Ответ должен быть коротким и структурированным. (Нельзя использовать форматирование текста ни в каком виде)";

                // Отправляем запрос в выбранные модели ИИ
                var aiResponses = await _responseProcessingModule.GetSelectedAIResponses(prompt, request.SelectedModels);

                // Возвращаем ответы ИИ
                return Ok(aiResponses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обработке запросов ИИ: {ex.Message}");
            }
        }
    }




    // Модель запроса
    public class AIRequest
    {
        public string UserInput { get; set; }
        public List<string> SelectedModels { get; set; }
    }

}