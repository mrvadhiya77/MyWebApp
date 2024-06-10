using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime DateOfOrder { get; set; }
        public DateTime DateOfShipping { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public DateTime DayOfPayment { get; set; }
        public DateTime DueDate { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? state { get; set; }
        public string? Postal { get; set; }

        public string? Name { get; set; }
    }
}
