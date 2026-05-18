using ePharma_asp_mvc.Data.Services;
using ePharma_asp_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;

        private readonly UserManager<ApplicationUser> _userManager;

        public PrescriptionController(IPrescriptionService prescriptionService, UserManager<ApplicationUser> userManager)
        {
            _prescriptionService = prescriptionService;
            _userManager = userManager;
        }

        // =========================
        // Upload Page
        // =========================

        public IActionResult Upload()
        {
            return View();
        }

        // =========================
        // Upload
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile imageFile, string notes, string address, string phoneNumber)
        {
            if (imageFile == null)
            {
                ModelState.AddModelError("", "Please upload image.");

                return View();
            }

            var userId = _userManager.GetUserId(User);

            await _prescriptionService.UploadPrescription(userId, imageFile, notes, address, phoneNumber);

            TempData["Success"] = "Prescription uploaded successfully.";

            ViewData["nama"] = "Prescription";
            return View("~/Views/Order/ThankYou.cshtml");
        }

        // =========================
        // My Prescriptions
        // =========================

        public async Task<IActionResult> MyPrescriptions()
        {
            var userId = _userManager.GetUserId(User);

            var prescriptions = await _prescriptionService.GetByUserIdAsync(userId);

            return View(prescriptions);
        }

        // =========================
        // Admin
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var prescriptions = await _prescriptionService.GetAllAsync();

            return View("AllPrescription", prescriptions);
        }


        // =========================
        // Prescription Details
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            // بنجيب الروشتة بالـ ID ونعمل Include لبيانات اليوزر
            var prescription = await _prescriptionService.GetByIdAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            // تأمين: لو مش أدمن، ما يشوفش روشتة يوزر تاني
            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && prescription.UserId != userId)
            {
                return Forbid();
            }

            return View("PrescriptionsDetails", prescription);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // بننادي السيرفيس وهي بتتصرف في المسح بالكامل
            var isDeleted = await _prescriptionService.DeleteAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Prescription deleted successfully.";
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var isUpdated = await _prescriptionService.UpdateStatusAsync(id, status);

            if (!isUpdated)
            {
                return NotFound();
            }

            TempData["Success"] = $"Prescription has been marked as {status} successfully.";

            return RedirectToAction(nameof(All));
        }
    }
}