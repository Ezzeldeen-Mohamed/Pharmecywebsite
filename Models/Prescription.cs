using ePharma_asp_mvc.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ePharma_asp_mvc.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string? Notes { get; set; }

        // NEW
        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public PrescriptionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        // User Relation
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}