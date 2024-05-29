using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.Controllers
{
    public class ApplyJobController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApplyJobController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", string sortOrder = "asc", string searchString = "")
        {
            // Get the data from the repository
            var applyJobs = _unitOfWork.ApplyJob.GetAll(includeProperties: "JobPost");
            // Apply searching
            if (!string.IsNullOrEmpty(searchString))
            {
                applyJobs = applyJobs.Where(job => job.Name.Contains(searchString) || job.Email.Contains(searchString));
            }
            // Apply sorting
            switch (sortBy)
            {
                case "Name":
                    applyJobs = sortOrder == "asc" ? applyJobs.OrderBy(job => job.Name) : applyJobs.OrderByDescending(job => job.Name);
                    break;
                case "PhoneNumber":
                    applyJobs = sortOrder == "asc" ? applyJobs.OrderBy(job => job.PhoneNumber) : applyJobs.OrderByDescending(job => job.PhoneNumber);
                    break;
                default:
                    applyJobs = applyJobs.OrderBy(job => job.Name);
                    break;
            }
            // Apply paging
            var totalItems = applyJobs.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedJobs = applyJobs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SortBy = sortBy;
            ViewBag.SearchString = searchString;

            return View(paginatedJobs);
        }
        public IActionResult Create(int jobPostId)
        {
            var jobPost = _unitOfWork.JobPost.Get(jobPostId);
            if (jobPost == null)
            {
                return NotFound();
            }

            var model = new Applyjob
            {
                JobPostId = jobPostId,
                JobPost = jobPost 
            };

            ViewData["JobTitle"] = jobPost.Title;
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Applyjob applyJob, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files/cv");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    applyJob.UploadCV = "/files/cv/" + uniqueFileName;
                }

                _unitOfWork.ApplyJob.Add(applyJob);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(applyJob);
        }

        public IActionResult DownloadCV(int id)
        {
            var applyJob = _unitOfWork.ApplyJob.Get(id);
            if (applyJob == null || string.IsNullOrEmpty(applyJob.UploadCV))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, applyJob.UploadCV.TrimStart('/'));
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
        }

        [HttpGet]
        public IActionResult GetPDFPath(int id)
        {
            var applyJob = _unitOfWork.ApplyJob.Get(id);
            if (applyJob == null || string.IsNullOrEmpty(applyJob.UploadCV))
            {
                return NotFound();
            }
            return Json(applyJob.UploadCV);
        }
    }
}
