using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisExample.Models;
using RedisExample.Repositories;

namespace RedisExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok( await _productRepository.GetAsync());
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            return Ok(await _productRepository.GetByIdAsync(Id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            return Created(string.Empty, await _productRepository.CreatAsync(product));
        }
    }
}
