﻿using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.Controllers
{
    public class ApplyJobController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        public ApplyJobController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var applyJobs = _unitOfWork.ApplyJob.GetAll(includeProperties: "JobPost").ToList();
            return View(applyJobs);
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
                var jobPost = _unitOfWork.JobPost.Get(applyJob.JobPostId);
                string applicantName = applyJob.Name ?? "Applicant";
                string applicantEmail = applyJob.Email ?? "no-reply@yourdomain.com";
                string jobTitle = jobPost?.Title ?? "the job position";
                string subject = "Application Received";
                string message = $"Dear {applicantName},<br><br>Thank you for applying for the {jobTitle} position. We have received your application and will review it shortly.<br><br>Best regards,<br>Application Tracking System";
                await _emailSender.SendEmailAsync(applicantEmail, subject, message);
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

        [HttpPost]
        public IActionResult AddToShortlist(int id)
        {
            var applyJob = _unitOfWork.ApplyJob.Get(id);
            if (applyJob == null)
            {
                return NotFound();
            }

            var shortlistedJob = new
            {
                Id = applyJob.Id,
                Name = applyJob.Name,
                PhoneNumber = applyJob.PhoneNumber,
                Email = applyJob.Email
            };

            return Json(shortlistedJob);
        }

        [HttpPost]
        public IActionResult RemoveFromShortlist(int id)
        {
            var applyJob = _unitOfWork.ApplyJob.Get(id);
            if (applyJob == null)
            {
                return NotFound();
            }

            return Json(new { Id = applyJob.Id });
        }
    }
}
