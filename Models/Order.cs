using ePharma_asp_mvc.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ePharma_asp_mvc.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime TimeOrdered { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? CustomerNotes { get; set; } // لو العميل عايز يسيب ملحوظة (مثلاً: "لو في بديل للدواء ده..")
        public string CustomAddress { get; set; } // لو العميل عايز يكتب عنوان خاص بيه للتوصيل (مثلاً: "سندبسط - بجوار المسجد..")

        //User
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        //OrderItems
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
