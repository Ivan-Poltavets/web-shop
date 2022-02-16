using OnlineShop.Models;

namespace OnlineShop.Repository
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProducts();
        public Product GetProduct(int id);
        public IEnumerable<Category> GetCategories();
        public Task<Category> GetCategory(int id);
        public string GetCategoryName(int id);
    }
}
