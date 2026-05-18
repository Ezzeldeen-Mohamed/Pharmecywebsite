using ePharma_asp_mvc.Data.Services;
using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly IShoppingCartsService _shoppingCartsService;
        private readonly IOrdersService _ordersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(
            IProductsService productsService,
            IShoppingCartsService shoppingCartsService,
            IOrdersService ordersService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _productsService = productsService;
            _shoppingCartsService = shoppingCartsService;
            _ordersService = ordersService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==============================
        // Shopping Cart
        // ==============================

        public async Task<IActionResult> ShoppingCart()
        {
            var userId = _userManager.GetUserId(User);

            var items =
                await _shoppingCartsService.GetAllItemsAsync(userId);

            return View(items.ToList());
        }

        [AllowAnonymous]
        public async Task<IActionResult> AddToShoppingCart(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "LogIn",
                    "ApplicationUser",
                    new { returnUrl = Request.Path });
            }

            var product =
                await _productsService.GetByID(id);

            if (product != null)
            {
                var userId = _userManager.GetUserId(User);

                await _shoppingCartsService
                    .AddItemToShoppingCart(product, userId);
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveFromShoppingCart(int id)
        {
            await _shoppingCartsService
                .RemoveItemFromShoppingCart(id);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            await _shoppingCartsService
                .IncreaseItemQuantity(id);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            await _shoppingCartsService
                .DecreaseItemQuantity(id);

            return RedirectToAction(nameof(ShoppingCart));
        }

        // ==============================
        // Checkout
        // ==============================

        public async Task<IActionResult> CheckOut()
        {
            var userId = _userManager.GetUserId(User);

            var items =
                await _shoppingCartsService.GetAllItemsAsync(userId);

            if (items == null || !items.Any())
            {
                return RedirectToAction(nameof(ShoppingCart));
            }

            return View(items.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(
            IFormFile prescriptionPhoto,
            string customAddress,
            string? customerNotes)
        {
            var userId = _userManager.GetUserId(User);

            var items =
                await _shoppingCartsService.GetAllItemsAsync(userId);

            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "Shopping cart is empty.");
                return View(items.ToList());
            }

            try
            {
                string prescriptionPhotoPath = "";

                // Check if prescription required
                //bool needsPrescription =
                //    items.Any(i => i.Product.NeedsPrescription);

                //if (needsPrescription && prescriptionPhoto == null)
                //{
                //    ModelState.AddModelError(
                //        "",
                //        "Prescription photo is required.");

                //    return View(items.ToList());
                //}

                // Upload image
                if (prescriptionPhoto != null &&
                    prescriptionPhoto.Length > 0)
                {
                    var allowedExtensions =
                        new[] { ".jpg", ".jpeg", ".png" };

                    var extension =
                        Path.GetExtension(
                            prescriptionPhoto.FileName)
                        .ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "",
                            "Only JPG and PNG files are allowed.");

                        return View(items.ToList());
                    }

                    string uploadsFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "prescriptions");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName =
                        Guid.NewGuid() +
                        "_" +
                        prescriptionPhoto.FileName;

                    string filePath =
                        Path.Combine(
                            uploadsFolder,
                            uniqueFileName);

                    using (var stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        await prescriptionPhoto
                            .CopyToAsync(stream);
                    }

                    prescriptionPhotoPath =
                        "/images/prescriptions/" +
                        uniqueFileName;
                }

                await _ordersService.CreateOrder(
                    userId,
                    items.ToList(),
                    prescriptionPhotoPath,
                    customAddress,
                    customerNotes);

                await _shoppingCartsService
                    .ClearShoppingCartItems(userId);

                ViewData["nama"] = "order";
                return View("ThankYou");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError(
                    "",
                    "An error occurred: " + ex.Message);

                return View(items.ToList());
            }
        }

        // ==============================
        // Orders
        // ==============================

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders =
                await _ordersService
                    .GetAllOrdersByUserAsync(userId);

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order =
                await _ordersService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order =
                await _ordersService.GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.UserId != userId)
            {
                return Forbid();
            }

            if (order.Status != OrderStatus.Pending)
            {
                TempData["Error"] =
                    "Order can no longer be cancelled.";

                return RedirectToAction(nameof(MyOrders));
            }

            await _ordersService.UpdateStatusAsync(
                id,
                OrderStatus.Cancelled);

            TempData["Success"] =
                "Order cancelled successfully.";

            return RedirectToAction(nameof(MyOrders));
        }
    }
}