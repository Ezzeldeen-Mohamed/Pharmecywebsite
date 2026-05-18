using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public interface IHealthServicesService
    {
        Task<IEnumerable<HealthService>> GetAllAsync();
        Task AddAsync(HealthService service);
        Task CreateServiceRequest(ServiceRequest request);
        Task<IEnumerable<ServiceRequest>> GetUserRequestsAsync(string userId);
        Task<IEnumerable<ServiceRequest>> GetAllRequestsAsync(); // للـ Admin
        Task UpdateRequestStatus(int requestId, ServiceRequestStatus newStatus);
        Task DeleteRequest(int requestId);
        Task<ServiceRequest> GetRequestDetails(int id);

    }
}
