using System;

namespace ProductsAndCategories.Models
{
    // The Many To Many Model / Table
    // One instance of this represents one relationship between one Product and one Category
    // A List of these represents Many relationships between Product and Category
    public class CategoryProduct
    {
        public int CategoryProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Keys
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties (not added to DB - must use .Include to get data)
        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}