using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SortedSetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        private string listKey = "sortedsetnames";

        public SortedSetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(3); // 1. VERİTABANINI KULLAN DİYORUZ.
        }
        public IActionResult Index()
        {
            Dictionary<string,double> list = new();
            if (db.KeyExists(listKey))
            {
                db.SortedSetScan(listKey).ToList().ForEach(x =>
                {
                    list.Add(x.Element, x.Score);
                });
            }
            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            db.SortedSetAdd(listKey, name, score);
            db.KeyExpire(listKey, DateTime.Now.AddMinutes(1));
            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem(string name)
        {
            db.SortedSetRemove(listKey, name);

            return RedirectToAction("Index");
        }
    }
}
