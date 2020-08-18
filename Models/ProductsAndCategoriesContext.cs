using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Models
{
    public class ProductsAndCategoriesContext : DbContext
    {
        public ProductsAndCategoriesContext(DbContextOptions options) : base(options) { }

        // for every model / entity that is going to be part of the db
        // the names of these properties will be the names of the tables in the db
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }

    }

}