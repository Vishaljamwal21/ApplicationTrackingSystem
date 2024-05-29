using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
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

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", string sortOrder = "asc", string searchString = "")
        {
            // Get the data from the repository
            var applyJobs = _unitOfWork.ApplyJob.GetAll();

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
                // Add more cases for other sortable columns as needed
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
            var model = new Applyjob
            {
                JobPostId = jobPostId
            };
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

        public IActionResult ExportToExcel()
        {
            var applyJobs = _unitOfWork.ApplyJob.GetAll();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Apply Jobs");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Phone Number";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Brief Yourself";

                int row = 2;
                foreach (var applyJob in applyJobs)
                {
                    worksheet.Cells[row, 1].Value = applyJob.Name;
                    worksheet.Cells[row, 2].Value = applyJob.PhoneNumber;
                    worksheet.Cells[row, 3].Value = applyJob.Email;
                    worksheet.Cells[row, 4].Value = applyJob.BriefYourself;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                byte[] excelFile = package.GetAsByteArray();

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ApplyJobs.xlsx");
            }
        }
    }
}
