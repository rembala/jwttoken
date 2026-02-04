using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetCoreRestApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _MemoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _MemoryCache = memoryCache;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            const string cache = "cachedTime";

            if (!_MemoryCache.TryGetValue(cache, out int cachedTime))
            {
                cachedTime = GetRandomNumber();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);

                _MemoryCache.Set(cache, cachedTime, cacheEntryOptions);
            }

            return Ok($"Random number -> {cachedTime}");
        }

        private static int GetRandomNumber()
        {
            return Random.Shared.Next();
        }
    }
}
