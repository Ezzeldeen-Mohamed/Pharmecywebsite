using ePharma_asp_mvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly AppDbContext _context;

        public ShoppingCartsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddItemToShoppingCart(Product product, string userId)
        {
            // 1. نجيب الكارت بتاع المستخدم أو نكريته لو مش موجود
            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId };
                await _context.ShoppingCarts.AddAsync(shoppingCart);
                await _context.SaveChangesAsync(); // بنسيف هنا عشان ناخد الـ ID الجديد
            }

            // 2. نشوف المنتج ده موجود في الكارت ولا لأ
            var shoppingCartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(n => n.ShoppingCartId == shoppingCart.Id && n.ProductId == product.Id);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ProductId = product.Id, // يفضل نبعت الـ ID بس أو الـ Object كامل عادي
                    ShoppingCartId = shoppingCart.Id,
                    Quantity = 1,
                };
                await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetAllItemsAsync(string userId)
        {
            // استخدمنا Include عشان نرجع بيانات المنتج (الاسم والسعر) مع العنصر
            return await _context.ShoppingCartItems
                .Where(n => n.ShoppingCart.UserId == userId)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task IncreaseItemQuantity(int id)
        {
            var item = await _context.ShoppingCartItems.FindAsync(id);
            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecreaseItemQuantity(int id)
        {
            var item = await _context.ShoppingCartItems.FindAsync(id);
            if (item != null)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    _context.ShoppingCartItems.Remove(item); // لو وصل لـ 0 نمسحه خالص

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemFromShoppingCart(int id)
        {
            var item = await _context.ShoppingCartItems.FindAsync(id);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearShoppingCartItems(string userId)
        {
            var items = await _context.ShoppingCartItems
                .Where(n => n.ShoppingCart.UserId == userId)
                .ToListAsync();

            if (items.Any())
            {
                _context.ShoppingCartItems.RemoveRange(items); // أسرع بكتير من الـ foreach
                await _context.SaveChangesAsync();
            }
        }
    }
}