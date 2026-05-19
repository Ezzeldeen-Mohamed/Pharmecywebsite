using System.ComponentModel.DataAnnotations;

namespace ePharma_asp_mvc.Models
{
    public class HealthService
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "اسم الخدمة")]
        public string Name { get; set; } // مثال: قياس ضغط، إعطاء حقنة
        [Display(Name = "الوصف")]
        public string Description { get; set; }
        [Display(Name = "السعر")]
        public decimal Price { get; set; }
    }
}
