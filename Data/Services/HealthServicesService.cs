using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public class HealthServicesService : IHealthServicesService
    {
        private readonly AppDbContext _context;

        public HealthServicesService(AppDbContext context)
        {
            _context = context;
        }

        // جلب كل الخدمات المتاحة (ضغط، سكر، حقن...) عشان تتعرض للمستخدم
        public async Task<IEnumerable<HealthService>> GetAllAsync()
        {
            return await _context.HealthServices.ToListAsync();
        }

        // إضافة خدمة جديدة - دي بيستخدمها الأدمن بس
        public async Task AddAsync(HealthService service)
        {
            await _context.HealthServices.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task CreateServiceRequest(ServiceRequest request)
        {
            // 1. نجيب بيانات الخدمة عشان السعر
            var service = await _context.HealthServices.FindAsync(request.ServiceId);
            if (service == null) throw new Exception("Service not found");

            // 2. نملأ البيانات اللي مش جاية من الفورم
            request.RequestTime = DateTime.Now;
            request.Status = ServiceRequestStatus.Pending;
            request.PriceAtTimeOfRequest = service.Price;

            // 3. الحفظ
            await _context.ServiceRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        // جلب طلبات مستخدم معين عشان يشوف الـ History بتاعه
        public async Task<IEnumerable<ServiceRequest>> GetUserRequestsAsync(string userId)
        {
            return await _context.ServiceRequests
                .Include(r => r.Service) // عشان نعرض اسم الخدمة مع الطلب   
                .Include(r => r.User) // عشان نعرض اسم الخدمة مع الطلب
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestTime)
                .ToListAsync();
        }

        // جلب كل الطلبات للأدمن عشان يتابع التنفيذ
        public async Task<IEnumerable<ServiceRequest>> GetAllRequestsAsync()
        {
            return await _context.ServiceRequests
                .Include(r => r.Service)
                .Include(r => r.User) // عشان الأدمن يعرف مين اللي طلب الخدمة
                .OrderByDescending(r => r.RequestTime)
                .ToListAsync();
        }

        public async Task UpdateRequestStatus(int requestId, ServiceRequestStatus newStatus)
        {
            var request = await _context.ServiceRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ServiceRequest> GetRequestDetails(int id)
        {
            return await _context.ServiceRequests
                .Include(r => r.Service)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteRequest(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request != null)
            {
                _context.ServiceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }

    }
}
