using System.ComponentModel.DataAnnotations;

namespace ePharma_asp_mvc.Data.ViewModels
{
    public class FilterProductsViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;

        // Filter
        public string Name { get; set; }
        public string Description { get; set; }

        // خليناها decimal عشان تتوافق مع الـ Model 
        // وخليناها Nullable عشان لو المستخدم مسابش قيمة السعر فاضية في البحث
        public decimal? Price { get; set; }

        // إضافة اختيار الترتيب عشان يوصل للـ Service
        public string SortOrder { get; set; }
    }

    public class GetProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "صورة المنتج مطلوبة")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "وصف المنتج مطلوب")]
        public string Description { get; set; }

        [Required(ErrorMessage = "سعر المنتج مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal Price { get; set; }
    }
}