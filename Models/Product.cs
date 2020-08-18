using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductsAndCategories.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least {1} characters.")] // {1} inserts whatever the MinLength value is
        public string Name { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least {1} characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "please enter a value that is at least {1}.")]
        public decimal? Price { get; set; } // this needs to be nullable (the question mark) in order for the required message to appear
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Keys

        // Navigation Properties (not added to DB - must use .Include for to get data)
        public List<CategoryProduct> CategoryProducts { get; set; }
    }
}