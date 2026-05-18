namespace ePharma_asp_mvc.Models.Enums
{
    public enum OrderStatus
    {
        Pending,     // قيد المراجعة
        Completed,   // تم التأكيد
        Accepted,
        Cancelled    // ملغي
    }
    public enum ServiceRequestStatus
    {
        Pending,    // الطلب واصل للصيدلية
        Accepted,   // الصيدلي وافق وهيطلع للمريض
        Completed,  // الخدمة تمت بنجاح
        Cancelled   // تم الإلغاء
    }
    public enum PrescriptionStatus
    {
        Pending,    // الطلب واصل للصيدلية
        Accepted,   // الصيدلي وافق وهيطلع للمريض
        Completed,  // الخدمة تمت بنجاح
        Cancelled   // تم الإلغاء
    }

}
