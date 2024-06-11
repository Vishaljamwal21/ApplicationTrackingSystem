using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ApplicationTrackingSystem.Controllers
{
    public class JobPostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobPostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var jobPosts = _unitOfWork.JobPost.GetAll(); 
            ViewBag.TodayDate = DateTime.Today.ToString("MMMM dd, yyyy");
            return View(jobPosts);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JobPost jobPost)
        {            
                int createdBy = 123;
                jobPost.DatePosted = DateTime.UtcNow;
                jobPost.CreatedBy = createdBy.ToString();
                jobPost.CreatedAt = DateTime.Now;

                _unitOfWork.JobPost.Add(jobPost);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(JobPost jobPost)
        {         
                _unitOfWork.JobPost.Update(jobPost);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));           
        }
        public IActionResult Edit(int id)
        {
            var jobPost = _unitOfWork.JobPost.Get(id);
            if (jobPost == null)
            {
                return NotFound();
            }
            return View(jobPost);
        }
        public IActionResult Delete(int id)
        {
            var jobPost = _unitOfWork.JobPost.Get(id);
            if (jobPost == null)
            {
                return NotFound();
            }
            return View(jobPost);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var jobPost = _unitOfWork.JobPost.Get(id);
            if (jobPost == null)
            {
                return NotFound();
            }
            _unitOfWork.JobPost.Remove(jobPost);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
