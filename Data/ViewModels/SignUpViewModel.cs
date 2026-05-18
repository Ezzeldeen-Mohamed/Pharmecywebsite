using System.ComponentModel.DataAnnotations;

namespace ePharma_asp_mvc.Data.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }
        [Required(ErrorMessage = "رقم الموبايل مطلوب")]
        public string PhoneNumber { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تأكيد كلمة المرور")]
        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        public string ConfirmPassword { get; set; }
    }
}
