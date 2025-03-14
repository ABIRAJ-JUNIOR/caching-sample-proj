using caching_sample_proj.Models;

namespace caching_sample_proj.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    }
    public class ProductService : IProductService
    {

        // Simulating database access with delay
        private readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200, Category = "Electronics" },
            new Product { Id = 2, Name = "Smartphone", Price = 800, Category = "Electronics" },
            new Product { Id = 3, Name = "Headphones", Price = 200, Category = "Electronics" },
            new Product { Id = 4, Name = "T-Shirt", Price = 25, Category = "Clothing" },
            new Product { Id = 5, Name = "Jeans", Price = 50, Category = "Clothing" },
            new Product { Id = 6, Name = "Coffee Maker", Price = 150, Category = "Home" },
            new Product { Id = 7, Name = "Blender", Price = 80, Category = "Home" }
        };


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // Simulate database delay
            await Task.Delay(1000);
            Console.WriteLine($"[{DateTime.Now}] Database hit: GetAllProducts");
            return _products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            // Simulate database delay
            await Task.Delay(300);
            Console.WriteLine($"[{DateTime.Now}] Database hit: GetProductById({id})");
            return _products.FirstOrDefault(p => p.Id == id)!;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            // Simulate database delay
            await Task.Delay(700);
            Console.WriteLine($"[{DateTime.Now}] Database hit: GetProductsByCategory({category})");
            return _products.Where(p => p.Category == category);
        }
    }
}
