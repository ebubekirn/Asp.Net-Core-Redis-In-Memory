using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        
        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0); // 0. VERİTABANINI KULLAN DİYORUZ.
        }
        public IActionResult Index()
        {
            db.StringSet("nameapi", "fatih çakıroğlu");
            db.StringSet("ziyaretciapi", 100);
            return View();
        }

        public IActionResult Show()
        {
            //var value = db.StringGet("nameapi");
            //var value = db.StringGetRange("nameapi", 0, 3);
            var value = db.StringLength("nameapi");
            //db.StringIncrement("ziyaretciapi", 10);
            //var count = db.StringDecrementAsync("ziyaretciapi", 1).Result; // async olarak çalışıp geriye değer döndürsün istersek vir değişkene atarız.
            db.StringDecrementAsync("ziyaretciapi", 10).Wait(); // Geriye değer döndürmesiyle ilgilenmiyorsak eait yazmamız yeterlidir.

            //if (value.HasValue) 
            //{
            //    ViewBag.value = value.ToString();
            //}
            ViewBag.value = value.ToString();

            return View();
        }
    }
}
