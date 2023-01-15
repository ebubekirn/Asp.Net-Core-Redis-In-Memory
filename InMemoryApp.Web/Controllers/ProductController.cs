using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        // ------------ Data cache leme için oluşturulacaklar. ------------
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        // ------------------------------------------------------------------

        public IActionResult Index()
        {
            // Set edilecek key in memory de olup olmaması durumu kontrol ediliyor.
            #region 11. Ders Memory Kontrol 1. YOL
            /*
            if (string.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString()); // Tarih bilgisini cache ye set ettim.
            }
            */
            #endregion

            // 2. YOL

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            //options.AbsoluteExpiration = DateTime.Now.AddSeconds(10); // 10 saniye sonra cache teki datayı siliyor.

            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1); 

            options.SlidingExpiration = TimeSpan.FromSeconds(10); // 10 saniye içerisinde dataya erişirsem data ömrü her seferinde 10 sn oluyor. Fakat bir üst satırda absolute olarak 1 dk girerek maksimum 60 saniye cache kalsın dedik. Bu sayede cache yi güncel tutabiliriz. 
            // !! SlidingExpiration ile kesinlikle AbsoluteExpiration kullanılması önerilir.

            // Cache Priority
            options.Priority = CacheItemPriority.High; // Burada High, Low, Normal ve NeverRemove seçebiliyoruz. High önemli, Low önemsiz, Normal ortalama öncelik ve NeverRemove da asla demek. NeverRemove dersek ve yeni data eklersek; memory dolduğunda yeni cache eklersek exception atar.

            options.RegisterPostEvictionCallback((key, value, reason, state) => {
                _memoryCache.Set("callback", $"{key} -> {value} => sebep: {reason}");
            });

            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options); // Tarih bilgisini cache ye set ettim. options ta ise cache e ömür ataması yapıyoruz.

            Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };

            _memoryCache.Set<Product>("product:1", p); // Serialize işlemi otomatik olarak gerçekleşmiş oldu. Redis te bu işlem manuel olarak yapılacak.

            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.Remove("zaman"); // Cache içerisindeki zaman keyine sahip datayı memoryden siliyor.

            #region GetOrCreate 11. ders
            /*
            _memoryCache.GetOrCreate<string>("zaman", entry => // GetorCreate mantığı girilen key e karşılık gelen data varsa al yoksa da bu key adında cache oluştur ve içerisinde fonksiyondaki datayı ata şeklindedir.
            {
                //entry yazma sebebimiz ise; eğer ki zaman veri tipine karşılık key yoksa key oluşturacak ve ekstra bir şeyler yapmak istersek entry üzerinden yapacağız.
                return DateTime.Now.ToString();
            });
            */
            #endregion

            _memoryCache.TryGetValue("zaman", out string zamancache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.callback = callback;
            ViewBag.zaman = zamancache;

            ViewBag.product = _memoryCache.Get<Product>("product:1");

            //ViewBag.zaman = _memoryCache.Get<string>("zaman"); // Cache deki datayı key ismi üzerinden get ettim
            return View();
        }
    }
}
