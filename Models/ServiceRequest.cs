using ePharma_asp_mvc.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ePharma_asp_mvc.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        // ربط بالخدمة نفسها (حقنة، ضغط، إلخ)
        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public HealthService Service { get; set; }

        // ربط بالمريض
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        [Required(ErrorMessage = "العنوان مطلوب لتوصيل الخدمة")]
        public string Address { get; set; } // العنوان الـ string (سندبسط - بجوار المسجد..)

        [Required(ErrorMessage = "رقم التليفون مطلوب")]
        [Display(Name = "رقم التواصل")]
        public string ContactPhoneNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceAtTimeOfRequest { get; set; } // السعر وقت الطلب عشان لو اتغير في الجدول الأصلي

        public DateTime RequestTime { get; set; } = DateTime.Now;

        public string? Notes { get; set; } // لو المريض عايز يسيب ملحوظة (مثلاً: "الحقنة عضل")
    }
}