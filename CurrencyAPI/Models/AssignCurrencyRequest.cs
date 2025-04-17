﻿using System.ComponentModel.DataAnnotations;

namespace CurrencyAPI.Models
{
    public class AssignCurrencyRequest
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must contain only 3 English letters.")]
        public string Currency { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be a positive number.")]
        public decimal Value { get; set; }
    }
}
