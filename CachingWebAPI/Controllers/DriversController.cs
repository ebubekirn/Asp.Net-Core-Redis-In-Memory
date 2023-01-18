﻿using CachingWebAPI.Data;
using CachingWebAPI.Models;
using CachingWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriversController : Controller
    {
        private readonly ILogger<DriversController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;

        public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDbContext context)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context; 
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get()
        {
            // check cache data
            var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");

            if(cacheData != null && cacheData.Count() > 0)
                return Ok(cacheData);

            cacheData = await _context.Drivers.ToListAsync();

            // Set expiry tine
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, expiryTime);

            return Ok(cacheData);
        }

        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post(Driver value)
        {
            var addedObj = await _context.Drivers.AddAsync(value);

            var expiryTime = DateTimeOffset.Now.AddSeconds(30);

            _cacheService.SetData<Driver>($"driver{value.Id}", addedObj.Entity, expiryTime);

            await _context.SaveChangesAsync();

            return Ok(addedObj.Entity);
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int Id)
        {
            var exist = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == Id);

            if (exist != null)
            {
                _context.Remove(exist);
                _cacheService.RemoveData($"driver{Id}");
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();
        }
     
    }
}
