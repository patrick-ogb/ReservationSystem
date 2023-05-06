﻿using System.ComponentModel.DataAnnotations;

namespace ReservationSyste.ViewModels
{
    public class PaymentOption
    {
        public int PaymentOptionId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public decimal Amount { get; set; }

    }
}
