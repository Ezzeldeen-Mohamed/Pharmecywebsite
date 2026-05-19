using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePharma_asp_mvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
        [Display(Name = "الاسم الأول")]
        public string FirstName { get; set; }
        [Display(Name = "الاسم الأخير")]
        public string LastName { get; set; }
        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [Required(ErrorMessage = "رقم الموبايل مطلوب")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "رقم موبايل  غير صحيح")]
        public override string PhoneNumber { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
