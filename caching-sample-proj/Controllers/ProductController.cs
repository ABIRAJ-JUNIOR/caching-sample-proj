using caching_sample_proj.Models;
using caching_sample_proj.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace caching_sample_proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public ProductController(IProductService productService, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _productService = productService;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        // No caching - for comparison
        [HttpGet("nocache")]
        public async Task<IActionResult> GetAllProductsNoCache()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // Memory Cache Example
        [HttpGet("memory-cache")]
        public async Task<IActionResult> GetAllProductsMemoryCache()
        {
            string cacheKey = "AllProducts";

            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product>? products))
            {
                // Cache miss - fetch from service
                products = await _productService.GetAllProductsAsync();

                // Set cache options
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetPriority(CacheItemPriority.Normal);

                // Save to cache
                _memoryCache.Set(cacheKey, products, cacheOptions);
                Console.WriteLine($"[{DateTime.Now}] Cache miss: {cacheKey}");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] Cache hit: {cacheKey}");
            }

            return Ok(products);
        }

        // Distributed Cache Example
        [HttpGet("distributed-cache")]
        public async Task<IActionResult> GetAllProductsDistributedCache()
        {
            string cacheKey = "AllProducts_Distributed";
            IEnumerable<Product> products;

            var cachedProducts = await _distributedCache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedProducts))
            {
                // Cache miss - fetch from service
                products = await _productService.GetAllProductsAsync();

                // Serialize and store in distributed cache
                string serializedProducts = JsonSerializer.Serialize(products);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await _distributedCache.SetStringAsync(cacheKey, serializedProducts, options);
                Console.WriteLine($"[{DateTime.Now}] Distributed cache miss: {cacheKey}");
            }
            else
            {
                // Cache hit - deserialize from cache
                products = JsonSerializer.Deserialize<IEnumerable<Product>>(cachedProducts)!;
                Console.WriteLine($"[{DateTime.Now}] Distributed cache hit: {cacheKey}");
            }

            return Ok(products);
        }


        // Cache by ID example
        [HttpGet("memory-cache/{id}")]
        public async Task<IActionResult> GetProductByIdMemoryCache(int id)
        {
            string cacheKey = $"Product_{id}";

            if (!_memoryCache.TryGetValue(cacheKey, out Product? product))
            {
                // Cache miss
                product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                    return NotFound();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _memoryCache.Set(cacheKey, product, cacheOptions);
                Console.WriteLine($"[{DateTime.Now}] Cache miss: {cacheKey}");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] Cache hit: {cacheKey}");
            }

            return Ok(product);
        }


    }
}
