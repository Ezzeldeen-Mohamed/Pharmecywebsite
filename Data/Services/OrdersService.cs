using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;

        public OrdersService(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateOrder(
            string userId,
            List<ShoppingCartItem> shoppingCartItems,
            string prescriptionPhotoPath,
            string customAddress,
            string? customerNotes = null)
        {
            var order = new Order
            {
                UserId = userId,
                TimeOrdered = DateTime.Now,
                Status = OrderStatus.Pending,
                CustomAddress = customAddress,
                PrescriptionPhoto = prescriptionPhotoPath,
                CustomerNotes = customerNotes,
                TotalPrice = shoppingCartItems.Sum(item =>
                    item.Product.Price * item.Quantity)
            };

            foreach (var item in shoppingCartItems)
            {
                var product =
                    await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                // Check Stock
                if (product.Stock < item.Quantity)
                {
                    throw new Exception(
                        $"Not enough stock for {product.Name}");
                }

                // Decrease Stock
                product.Stock -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = (double)product.Price
                });
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersByUserAsync(string userId)
        {
            return await _context.Orders
                .Where(n => n.UserId == userId)
                .OrderByDescending(o => o.TimeOrdered)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersForAdminAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.TimeOrdered)
                .ToListAsync();
        }
    }
}