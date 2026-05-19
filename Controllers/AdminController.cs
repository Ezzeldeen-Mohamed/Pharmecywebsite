using ePharma_asp_mvc.Data;
using ePharma_asp_mvc.Data.Services;
using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOrdersService _ordersService;
        private readonly IPrescriptionService _prescriptionService;

        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager, IOrdersService serive, IPrescriptionService prescriptionService)
        {
            _context = context;
            _userManager = userManager;
            _ordersService = serive;
            _prescriptionService = prescriptionService;
        }

        // =========================================
        // Dashboard
        // =========================================

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts =
                await _context.Products.CountAsync();

            ViewBag.TotalOrders =
                await _context.Orders.CountAsync();

            ViewBag.TotalUsers =
                await _context.Users.CountAsync();

            ViewBag.PendingOrders =
                await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Pending);

            ViewBag.CompletedOrders =
                await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Completed);

            ViewBag.TotalRevenue =
                await _context.Orders
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            ViewBag.TotalPrescriptions = await _prescriptionService.GetAllAsync();

            return View();
        }

        // =========================================
        // Products Management
        // =========================================

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _ordersService.GetAllOrdersForAdminAsync();
            return View("Orders", orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _ordersService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            await _ordersService.UpdateStatusAsync(id, status);
            TempData["Success"] = "Order status updated successfully.";
            return RedirectToAction(nameof(OrderDetails), new { id = id });
        }

        // =========================================
        // Users Management
        // =========================================

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            return View(users);
        }

        // =========================================
        // Delete User
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users));
        }

        // =========================================
        // Health Service Requests
        // =========================================

        //public async Task<IActionResult> ServiceRequests()
        //{
        //    var requests = await _context.ServiceRequests
        //        .Include(r => r.User)
        //        .Include(r => r.Service)
        //        .OrderByDescending(r => r.RequestTime)
        //        .ToListAsync();

        //    return View(requests);
        //}


    }
}