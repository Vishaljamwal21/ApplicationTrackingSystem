using ApplicationTrackingSystem.DataAccess.Data.Repository;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Authorization;
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
            return View(jobPosts);
        }

        public IActionResult Create()
        {
            return View();
        }

    [Authorize(Roles = "HR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(JobPost jobPost)
    {
        if (ModelState.IsValid)
        {
            var currentUser = User.Identity.Name;
            jobPost.DatePosted = DateTime.UtcNow;
            jobPost.CreatedBy = currentUser;
            jobPost.CreatedAt = DateTime.Now;

            _unitOfWork.JobPost.Add(jobPost);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        return View(jobPost);
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

        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                var jobPost = _unitOfWork.JobPost.Get(id);
                if (jobPost != null)
                {
                    _unitOfWork.JobPost.Remove(jobPost);
                    _unitOfWork.Save();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Job post not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
