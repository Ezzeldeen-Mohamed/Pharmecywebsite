using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ePharma_asp_mvc.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "السعر")]
        public decimal Price { get; set; }

        [Display(Name = "صورة المنتج")]
        public string? ImageUrl { get; set; } // هنخزن هنا مسار الصورة بس

        [Required(ErrorMessage = "الكمية المتاحة مطلوبة")]
        [Display(Name = "الكمية")]
        public int Stock { get; set; }

        [Display(Name = "الفئة")]
        public string Category { get; set; }

    }
}
