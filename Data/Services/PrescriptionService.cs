using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _environment;

        public PrescriptionService(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================
        // Upload Prescription
        // =========================

        public async Task UploadPrescription(
            string userId,
            IFormFile imageFile,
            string notes,
            string address,
            string phoneNumber)
        {
            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "prescriptions");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName =
                Guid.NewGuid() +
                "_" +
                imageFile.FileName;

            string filePath =
                Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

            using (var stream =
                   new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var prescription = new Prescription
            {
                UserId = userId,

                Notes = notes,
                Address = address,
                PhoneNumber = phoneNumber,

                ImagePath =
                    "/images/prescriptions/" +
                    uniqueFileName
            };

            _context.Prescriptions.Add(prescription);

            await _context.SaveChangesAsync();
        }

        // =========================
        // All Prescriptions
        // =========================

        public async Task<IEnumerable<Prescription>>
            GetAllAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }



        // =========================
        // User Prescriptions
        // =========================

        public async Task<IEnumerable<Prescription>>
            GetByUserIdAsync(string userId)
        {
            return await _context.Prescriptions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }


        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return false;

            // محاولة تحويل الـ string القادم من الـ View إلى الـ Enum الخاص بالحالة
            if (Enum.TryParse<PrescriptionStatus>(status, true, out var parsedStatus))
            {
                prescription.Status = parsedStatus;
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // في حال كانت قيمة الـ status الممررة غير مطابقة للـ Enum
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prescription == null)
            {
                return false;
            }
            // حذف الصورة من السيرفر
            string filePath =
                Path.Combine(
                    _environment.WebRootPath,
                    prescription.ImagePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
