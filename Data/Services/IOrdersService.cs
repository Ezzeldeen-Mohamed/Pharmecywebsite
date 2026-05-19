using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public interface IOrdersService
    {
        // إنشاء طلب جديد
        Task CreateOrder(string userId, List<ShoppingCartItem> shoppingCartItems, string customAddress, string? customerNotes = null);

        // جلب طلبات يوزر معين
        Task<IEnumerable<Order>> GetAllOrdersByUserAsync(string userId);

        // جلب كل الطلبات للأدمن
        Task<IEnumerable<Order>> GetAllOrdersForAdminAsync();

        // جلب تفاصيل طلب واحد (شاملة المنتجات واليوزر)
        Task<Order> GetOrderByIdAsync(int orderId);

        // تحديث حالة الطلب
        Task UpdateStatusAsync(int orderId, OrderStatus newStatus);
    }
}
