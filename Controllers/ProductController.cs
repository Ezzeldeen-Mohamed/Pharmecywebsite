using ePharma_asp_mvc.Data.Services;
using ePharma_asp_mvc.Data.ViewModels;
using ePharma_asp_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductsService _productsService;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(
            IProductsService productsService,
            IWebHostEnvironment webHostEnvironment)
        {
            _productsService = productsService;
            _webHostEnvironment = webHostEnvironment;
        }

        // =========================================
        // All Products
        // =========================================

        public async Task<IActionResult> Index()
        {
            var products = await _productsService.GetAllAsync();

            return View("/Views/Home/Shop.cshtml", products);
        }

        // =========================================
        // Product Details
        // =========================================

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productsService.GetByID(id);

            if (product == null)
            {
                return NotFound();
            }

            return View("/Views/Home/SingleProduct.cshtml", product);
        }

        // =========================================
        // Search Products
        // =========================================

        public async Task<IActionResult> Search(string searchString)
        {
            var products =
                await _productsService.Search(searchString);

            ViewBag.SearchString = searchString;

            return View("Index", products);
        }

        // =========================================
        // Sort Products
        // =========================================

        public async Task<IActionResult> Sort(string order)
        {
            var products =
                await _productsService.Sort(order);

            return View("Index", products);
        }

        // =========================================
        // Filter Products By Price
        // =========================================

        public async Task<IActionResult> Filter(
            int? minPrice,
            int? maxPrice)
        {
            var products =
                await _productsService.Filter(minPrice, maxPrice);

            return View("Index", products);
        }

        // =========================================
        // Advanced Filter + Pagination
        // =========================================

        public async Task<IActionResult> FilterAll(
            FilterProductsViewModel query)
        {
            var result =
                await _productsService.FilterAllAsync(query);

            return View("Shop", result);
        }

        // =========================================
        // Create Product
        // =========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Product product,
            IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", product);
            }

            if (imageFile == null)
            {
                ModelState.AddModelError("", "Image is required");
                return View(product);
            }

            // Upload Image
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                string uniqueFileName =
                    Guid.NewGuid() + "_" + imageFile.FileName;

                string filePath =
                    Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImageUrl =
                    "/images/products/" + uniqueFileName;
            }

            await _productsService.AddProduct(product);

            return RedirectToAction(nameof(Index));
        }

        // =========================================
        // Edit Product
        // =========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product =
                await _productsService.GetByID(id);

            if (product == null)
            {
                return NotFound();
            }

            return View("/Views/Product/Edit.cshtml", product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    Product product,
    IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("/Views/Product/Edit.cshtml", product);
            }

            var oldProduct =
                await _productsService.GetByID(id);

            if (oldProduct == null)
                return NotFound();

            // =========================
            // UPDATE FIELDS مباشرة
            // =========================
            oldProduct.Name = product.Name;
            oldProduct.Description = product.Description;
            oldProduct.Price = product.Price;
            oldProduct.Stock = product.Stock;
            oldProduct.Category = product.Category;

            // =========================
            // IMAGE
            // =========================
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName =
                    Guid.NewGuid() + "_" + imageFile.FileName;

                string filePath =
                    Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                oldProduct.ImageUrl =
                    "/images/products/" + uniqueFileName;
            }

            await _productsService.Update(id, oldProduct);

            return RedirectToAction(nameof(Index));
        }

        // =========================================
        // Delete Product
        // =========================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productsService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}