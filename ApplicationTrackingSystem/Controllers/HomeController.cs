using ApplicationTrackingSystem.DataAccess.Data.Repository;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using ApplicationTrackingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApplicationTrackingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
            
        }

        public IActionResult Index()
        {
            var jobPosts = _unitOfWork.JobPost.GetAll();
            return View(jobPosts);
           
        }

        public IActionResult Detail(int id)
        {
            var jobPosts = _unitOfWork.JobPost.Get(id);
            return View(jobPosts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
