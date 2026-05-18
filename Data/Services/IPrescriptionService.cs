using ePharma_asp_mvc.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public interface IPrescriptionService
    {
        Task UploadPrescription(string userId, IFormFile imageFile, string notes, string address, string phoneNumber);
        Task<IEnumerable<Prescription>> GetAllAsync();
        Task<IEnumerable<Prescription>> GetByUserIdAsync(string userId);
        Task<Prescription> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}
