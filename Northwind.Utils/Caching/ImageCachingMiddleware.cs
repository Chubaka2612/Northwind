using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Northwind.Utils.Caching
{
    public class ImageCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ImageCachingMiddleware> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _cacheDirectory;
        private readonly int _maxCacheCount;
        private readonly TimeSpan _cacheExpirationTime;

        public ImageCachingMiddleware(RequestDelegate next, ILogger<ImageCachingMiddleware> logger, IMemoryCache cache, ImageCachingOptions imageCachingOptions)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _cacheDirectory = imageCachingOptions.CacheDirectory;
            _maxCacheCount = imageCachingOptions.MaxCacheCount;
            _cacheExpirationTime = imageCachingOptions.CacheExpirationTime;

            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cacheKey = GenerateCacheKey(context.Request);

            if (_cache.TryGetValue(cacheKey, out string cachedFilePath) && File.Exists(cachedFilePath))
            {
                // Return cached image
                context.Response.ContentType = GetContentTypeFromFileName(cachedFilePath);
                context.Response.Headers.Add("Content-Disposition", "inline");
                await context.Response.SendFileAsync(cachedFilePath);
                _logger.LogInformation("Served image from cache: {CacheKey}", cacheKey);
                return;
            }

            // Capture the response
            var originalBodyStream = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;
                await _next(context);

                if (IsImageContentType(context.Response.ContentType))
                {
                    if (memoryStream.Length > 0)
                    {
                        memoryStream.Position = 0;

                        // Save image to cache
                        var filePath = Path.Combine(_cacheDirectory, cacheKey + GetFileExtension(context.Response.ContentType));
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                        }

                        // Manage cache limits
                        ManageCacheLimits();

                        // Set expiration for cached image
                        _cache.Set(cacheKey, filePath, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = _cacheExpirationTime,
                            PostEvictionCallbacks =
                        {
                            new PostEvictionCallbackRegistration
                            {
                                EvictionCallback = (key, value, reason, state) =>
                                {
                                    if (File.Exists(filePath))
                                    {
                                        File.Delete(filePath);
                                    }
                                }
                            }
                        }
                        });

                        _logger.LogInformation("Cached new image: {FilePath}", filePath);
                    }

                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
                else
                {
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            context.Response.Body = originalBodyStream;
        }

        private static string GenerateCacheKey(HttpRequest request)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Path + request.QueryString));
        }

        private static string GetFileExtension(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                _ => ".img"
            };
        }

        private static bool IsImageContentType(string contentType)
        {
            return contentType?.StartsWith("image/") == true;
        }

        private string GetContentTypeFromFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }

        private void ManageCacheLimits()
        {
            var files = Directory.GetFiles(_cacheDirectory);
            if (files.Length > _maxCacheCount)
            {
                var filesToDelete = files
                    .OrderBy(File.GetLastAccessTime)
                    .Take(files.Length - _maxCacheCount);
                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                }
            }
        }
    }
}
