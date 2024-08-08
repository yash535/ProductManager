using Microsoft.EntityFrameworkCore;
using ProductManageApi.Data;
using ProductManageApi.Models;

namespace ProductManageApi.Repositories
{
    public class ProductRepository
    {
        private readonly ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DecrementStockAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null && product.StockAvailable >= quantity)
            {
                product.StockAvailable -= quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> AddToStockAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.StockAvailable += quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
