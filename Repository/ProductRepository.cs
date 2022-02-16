using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationContext _context;
        public ProductRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetCategories()
        {
            var categories = _context.Categories.ToList();
            return categories;
        }

        public async Task<Category> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category;
        }

        public Product GetProduct(int id)
        {
            var product = _context.Products.Find(id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }
        public string GetCategoryName(int id)
        {
            var category = _context.Categories.Find(id);
            return category.Name;
        }

    }
}
