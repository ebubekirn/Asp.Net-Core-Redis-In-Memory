using RedisExample.Models;

namespace RedisExample.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAsync();
        Task<Product> GetByIdAsync(int Id);
        Task<Product> CreatAsync(Product product);

    }
}
