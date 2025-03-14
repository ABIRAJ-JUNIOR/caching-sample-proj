using caching_sample_proj.Models;
using caching_sample_proj.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

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
    }
}
