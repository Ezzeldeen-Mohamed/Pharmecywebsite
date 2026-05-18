using ePharma_asp_mvc.Data.Services;
using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    public class HealthServicesController : Controller
    {
        private readonly IHealthServicesService _healthService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HealthServicesController(
            IHealthServicesService healthService,
            UserManager<ApplicationUser> userManager)
        {
            _healthService = healthService;
            _userManager = userManager;
        }

        // عرض كل الخدمات
        public async Task<IActionResult> Index()
        {
            var services = await _healthService.GetAllAsync();
            return View(services);
        }

        // صفحة طلب الخدمة
        public IActionResult RequestService(int serviceId)
        {
            var request = new ServiceRequest
            {
                ServiceId = serviceId
            };

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, ServiceRequestStatus status)
        {
            try
            {
                await _healthService.UpdateRequestStatus(id, status);
                TempData["Success"] = "تم تحديث حالة الطلب بنجاح!";
            }
            catch (Exception)
            {
                TempData["Error"] = "حدث خطأ أثناء تحديث الحالة.";
            }

            return RedirectToAction("AllRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestService(ServiceRequest request)
        {
            // بنشيل الـ Navigation properties من الـ Validation عشان ميعملش Error
            ModelState.Remove("User");
            ModelState.Remove("Service");

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                // ربط الـ User الحالي بالطلب
                request.UserId = _userManager.GetUserId(User);

                // تنفيذ العملية من خلال الـ Service
                await _healthService.CreateServiceRequest(request);

                return RedirectToAction(nameof(MyRequests));
            }
            catch (Exception ex)
            {
                // لو حصل مشكلة (مثلاً الخدمة ممسوحة)
                ModelState.AddModelError("", "حدث خطأ أثناء تنفيذ الطلب: " + ex.Message);
                return View(request);
            }
        }

        // طلبات المستخدم
        public async Task<IActionResult> MyRequests()
        {
            var userId = _userManager.GetUserId(User);

            var requests =
                await _healthService.GetUserRequestsAsync(userId);

            return View(requests);
        }

        //الأدمن يشوف كل الطلبات
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllRequests()
        {
            var requests =
                await _healthService.GetAllRequestsAsync();

            return View("ServiceRequests", requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _healthService.GetRequestDetails(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int id)
        {
            await _healthService.UpdateRequestStatus(id, ServiceRequestStatus.Cancelled);
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // الحذف للأدمن فقط
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _healthService.DeleteRequest(id);
            return RedirectToAction(nameof(AllRequests));
        }
    }
}
