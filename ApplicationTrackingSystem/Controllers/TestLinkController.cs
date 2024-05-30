using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTrackingSystem.Controllers
{
    public class TestLinkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveSelectedLink([FromBody] ScheduleTestModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SelectedLink"] = model.SelectedLink;
                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false });
            }
        }
    }
}
