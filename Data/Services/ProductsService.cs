using ePharma_asp_mvc.Data.ViewModels;
using ePharma_asp_mvc.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public class ProductsService : IProductsService
    {
        private readonly AppDbContext _context;

        public ProductsService(AppDbContext context)
        {
            _context = context;
        }

        // 1. إضافة منتج
        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // 2. تعديل منتج (كانت ناقصة)
        public async Task Update(int id, Product newProduct)
        {
            var dbProduct = await _context.Products.FirstOrDefaultAsync(n => n.Id == id);

            if (dbProduct != null)
            {
                dbProduct.Name = newProduct.Name;
                dbProduct.Description = newProduct.Description;
                dbProduct.Price = newProduct.Price;
                dbProduct.ImageUrl = newProduct.ImageUrl;
                dbProduct.Stock = newProduct.Stock;
                dbProduct.Category = newProduct.Category;
                // ضيف أي خواص تانية لو موجودة في الموديل

                await _context.SaveChangesAsync();
            }
        }

        // 3. مسح منتج (كانت ناقصة)
        public async Task Delete(int id)
        {
            var result = await _context.Products.FirstOrDefaultAsync(n => n.Id == id);
            if (result != null)
            {
                _context.Products.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        // 4. جلب منتج بالـ ID
        public async Task<Product> GetByID(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        // 5. جلب كل المنتجات
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // 6. البحث (تم تحسينه ليكون Server-side)
        public async Task<IEnumerable<Product>> Search(string searchString)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            return await query.ToListAsync();
        }

        // 7. الترتيب (تم تحسينه ليكون أسرع)
        public async Task<IEnumerable<Product>> Sort(string order)
        {
            var query = _context.Products.AsQueryable();

            query = (order?.ToLower()) switch
            {
                "nameasc" => query.OrderBy(p => p.Name),
                "namedesc" => query.OrderByDescending(p => p.Name),
                "priceasc" => query.OrderBy(p => p.Price),
                "pricedesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Id),
            };

            return await query.ToListAsync();
        }

        // 8. الفلترة بالسعر
        public async Task<IEnumerable<Product>> Filter(int? minPrice, int? maxPrice)
        {
            return await _context.Products
                .Where(p => (minPrice == null || p.Price >= minPrice)
                         && (maxPrice == null || p.Price <= maxPrice))
                .ToListAsync();
        }

        // 9. الفلترة المتقدمة مع الـ Pagination
        public async Task<PagedResult<GetProductViewModel>> FilterAllAsync(FilterProductsViewModel query)
        {
            query.PageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var queryable = _context.Products
                .AsNoTracking()
                .AsQueryable();

            // Filter By Name
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var name = query.Name.ToLower();

                queryable = queryable.Where(p =>
                    p.Name.ToLower().Contains(name));
            }

            // Filter By Description
            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                var description = query.Description.ToLower();

                queryable = queryable.Where(p =>
                    p.Description.ToLower().Contains(description));
            }

            // Filter By Price
            if (query.Price > 0)
            {
                queryable = queryable.Where(p =>
                    p.Price == query.Price);
            }

            var totalCount = await queryable.CountAsync();

            var products = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new GetProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .ToListAsync();

            return new PagedResult<GetProductViewModel>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
    }
}