using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsAndCategories.Models
{
    [NotMapped]
    public class CategoryPage
    {
        public Category Category { get; set; }
        public List<Product> UnrelatedProducts { get; set; }
        public CategoryProduct NewRelationship { get; set; } = new CategoryProduct();
    }
}