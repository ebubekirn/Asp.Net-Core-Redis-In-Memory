using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            Product product = new Product { Id = 1, Name = "Kalem", Price = 100 };
            string jsonproduct = JsonConvert.SerializeObject(product);

            // Byte çevirme ile set işlemleri
            Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonproduct);
            _distributedCache.Set("product:1", byteproduct);

            // JSON serialize işlemleri ile set leme işlemleri 
            //await _distributedCache.SetStringAsync("product:1", jsonproduct, cacheEntryOptions);

            /* async set ve set işlemleri
            _distributedCache.SetString("names", "Fatih", cacheEntryOptions);
            await _distributedCache.SetStringAsync("surname", "nayman");
            */
            return View();
        }

        public IActionResult Show()
        {
            // JSON serialize ile yapılan işlemin get edilmesi
            //string jsonproduct = _distributedCache.GetString("product:1");

            // Byte serialize ile yapılan işlemin get edilmesi
            Byte[] byteproduct = _distributedCache.Get("product:1");
            string jsonproduct = Encoding.UTF8.GetString(byteproduct);

            // Byte ve Json da ortak olan kısım
            Product p = JsonConvert.DeserializeObject<Product>(jsonproduct);
            ViewBag.product = p;

            //string name = _distributedCache.GetString("names");
            //ViewBag.name = name;

            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("names");

            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/indir.jpg");

            byte[] imagebyte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("resim", imagebyte);

            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] resimbyte = _distributedCache.Get("resim");

            return File(resimbyte, "image/jpg");
        }
    }
}
