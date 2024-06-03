
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTrackingSystem.Controllers
{
    public class FormLinkController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FormLinkController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var formLinks = _unitOfWork.FormLink.GetAll();
            return View(formLinks);
        }

        [HttpGet]
        public IActionResult CreateOrEdit(int? id)
        {
            if (id == null)
            {
                return View(new FormLinks());
            }
            else
            {
                var formLink = _unitOfWork.FormLink.Get(id.Value);
                if (formLink == null)
                {
                    return NotFound();
                }
                return View(formLink);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(FormLinks formLink)
        {
            if (ModelState.IsValid)
            {
                if (formLink.Id == 0) // If Id is 0, it's a new form link
                {
                    _unitOfWork.FormLink.Add(formLink);
                }
                else // Otherwise, it's an existing form link being edited
                {
                    var existingFormLink = _unitOfWork.FormLink.Get(formLink.Id);
                    if (existingFormLink == null)
                    {
                        return NotFound();
                    }

                    existingFormLink.Title = formLink.Title;
                    existingFormLink.Links = formLink.Links;

                    _unitOfWork.FormLink.Update(existingFormLink);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(formLink);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var formLink = _unitOfWork.FormLink.Get(id);
            if (formLink == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.FormLink.Remove(formLink);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
        [HttpGet]
        public IActionResult GetFormLinks()
        {
            var formLinks = _unitOfWork.FormLink.GetAll();
            var result = formLinks.Select(fl => new { Id = fl.Id, Title = fl.Title, Links=fl.Links });
            return Json(result);
        }

    }
}
