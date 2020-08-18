using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Models;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private ProductsAndCategoriesContext db;
        public HomeController(ProductsAndCategoriesContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("Products");
        }

        // ViewBag Approach, see Categories method / action for how to do it without a ViewBag
        // ViewBag is used to pass this data so that the @model for the page can be a single Product for creating the product
        [HttpGet("/products")]
        public IActionResult Products()
        {
            ViewBag.products = db.Products.ToList();
            return View("Products");
        }

        [HttpPost("/products/create")]
        public IActionResult CreateProduct(Product newProduct)
        {
            if (ModelState.IsValid == false)
            {
                // since we are returning the view directly here, we need to provide whatever this view needs
                // each Method / Action has it's OWN ViewBag, it is not shared
                ViewBag.products = db.Products.ToList();
                return View("Products");
            }

            db.Products.Add(newProduct);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        // ViewBag approach, see Category method / action for how to do it without a ViewBag
        // ViewBag is used to pass in the product so that the @model can be used for the <form> creating a CategoryProduct relationship
        // {productId} is the route param that corresponds to the asp-route-productId in the Products.cshtml
        [HttpGet("/products/{productId}")]
        public IActionResult Product(int productId)
        {
            Product prod = db.Products
                .Include(product => product.CategoryProducts)
                // .ThenInclude is when you need to .Include something from the thing that was previously .Included
                // Rather than .Include another thing from the same Entity
                .ThenInclude(catProd => catProd.Category)
                .FirstOrDefault(p => p.ProductId == productId);

            if (prod == null)
            {
                return RedirectToAction("Products");
            }

            ViewBag.Product = prod;

            // .Where to keep only the categories where this product id is not already found in the many to many list
            // This could be done manually with foreach loops and .Add to a list only the ones that pass an if condition
            ViewBag.UnrelatedCategories = db.Categories
                .Include(cat => cat.CategoryProducts)
                .Where(cat => cat.CategoryProducts.Any(catProd => catProd.ProductId == prod.ProductId) == false).ToList();

            return View("Product");
        }

        [HttpPost("products/{productId}/relate")]
        public IActionResult RelateCategoryToProduct(int productId, CategoryProduct newCatProd)
        {
            // because the productId param is named the same as the prop in CategoryProduct it is auto assigned to the newCatProd
            // because of the asp-for on the <select> the categoryId is auto assigned to the newCatProd

            db.CategoryProducts.Add(newCatProd);
            db.SaveChanges();

            // new { paramName = paramValue } to pass in the arguments for Product method's parameters
            return RedirectToAction("Product", new { productId = productId });
        }

        // no ViewBag technique, using a <partial> view in the returned View
        [HttpGet("/categories")]
        public IActionResult Categories()
        {
            List<Category> categories = db.Categories.ToList();
            return View("Categories", categories);
        }

        // no ViewBag, form submission came from _newCategory <partial> view
        [HttpPost("categories/create")]
        public IActionResult CreateCategory(Category newCategory)
        {
            if (ModelState.IsValid == false)
            {
                // since we are returning the view directly here, we need to provide whatever this view needs
                // each Method / Action has it's OWN ViewBag, it is not shared
                List<Category> categories = db.Categories.ToList();
                return View("Categories", categories);
            }

            db.Categories.Add(newCategory);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }

        // no ViewBag technique, using a wrapper class to pass more data in the view model and using a <partial> view
        [HttpGet("/categories/{categoryId}")]
        public IActionResult Category(int categoryId)
        {
            Category cat = db.Categories
                .Include(category => category.CategoryProducts)
                // .ThenInclude is when you need to .Include something from the thing that was previously .Included
                // Rather than .Include another thing from the same Entity
                .ThenInclude(catProd => catProd.Product)
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (cat == null)
            {
                return RedirectToAction("Categories");
            }

            // .Where to keep only the products where this category id is not already found in the many to many list
            // This could be done manually with foreach loops and .Add to a list only the ones that pass an if condition
            List<Product> unrelatedProduct = db.Products
                .Include(prod => prod.CategoryProducts)
                .Where(prod => prod.CategoryProducts.Any(catProd => catProd.CategoryId == cat.CategoryId) == false).ToList();

            CategoryPage categoryPage = new CategoryPage()
            {
                Category = cat,
                UnrelatedProducts = unrelatedProduct
            };
            // set this Foreign Key (FK) since we know this one already but the product FK will be set from the <select> menu
            // categoryPage.NewRelationship.CategoryId = categoryId;

            return View("Category", categoryPage);
        }

        [HttpPost("/categories/{categoryId}/relate")]
        public IActionResult RelateProductToCategory(int categoryId, CategoryPage categoryPage)
        {
            // categoryPage.NewRelationship.ProductId has been auto filled out because of the asp-for on the <select>

            // CategoryId FK was not auto assigned because the FK property is nested in the categoryPage
            // It could have been auto assigned if we passed it through a hidden input with asp-for on it instead of a URL parameter
            categoryPage.NewRelationship.CategoryId = categoryId;
            db.CategoryProducts.Add(categoryPage.NewRelationship);
            db.SaveChanges();

            // new { paramName = paramValue } to pass in the arguments for Category method's parameters
            return RedirectToAction("Category", new { categoryId = categoryId });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
