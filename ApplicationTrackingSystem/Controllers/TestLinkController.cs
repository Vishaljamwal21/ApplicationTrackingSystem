using Microsoft.AspNetCore.Mvc;
using ApplicationTrackingSystem.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace ApplicationTrackingSystem.Controllers
{
    public class TestLinkController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TestLinkController> _logger;
        private readonly IConfiguration _configuration;

        public TestLinkController(IEmailSender emailSender, ILogger<TestLinkController> logger,IConfiguration configuration)
        {
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Test Response";
            ViewData["SelectedLink"] = _configuration.GetSection("EmbedLinks")["SelectedLink"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSelectedLink([FromBody] ScheduleTestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var testLink = model.SelectedLink;
                    foreach (var email in model.Email)
                    {
                        var emailMessage = $@"
                            Dear Candidate,

                            You have been scheduled for a {model.TestType} test on {model.TestDate} at {model.StartTime}. The test will last for {model.Duration} minutes.

                            Please complete the test using the following link: {testLink}

                            Best regards,
                            Your Company";
                        await _emailSender.SendEmailAsync(email, "Skill Test Schedule", emailMessage);
                    }

                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scheduling skill test");
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid model state" });
            }
        }
    }
}
