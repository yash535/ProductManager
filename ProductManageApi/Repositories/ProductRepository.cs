using Microsoft.EntityFrameworkCore;
using ProductManageApi.Data;
using ProductManageApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Product?> GetProductByIdAsync(int id)
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

        public async Task DeleteProductByIdAsync(int id)
        {
            Product? product = await GetProductByIdAsync(id);
            if (product is null) return;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> DecrementStockAsync(int id, int quantity)
        {
            var product = await GetProductByIdAsync(id);
            if (product is null)
            {
                return null;
            }

            if (product.StockAvailable < quantity)
            {
                return product.StockAvailable;
            }

            product.StockAvailable -= quantity;
            await _context.SaveChangesAsync();
            return product.StockAvailable;
        }

        public async Task<int?> AddToStockAsync(int id, int quantity)
        {
            var product = await GetProductByIdAsync(id);
            if (product is null)
            {
                return null;
            }

            product.StockAvailable += quantity;
            await _context.SaveChangesAsync();
            return product.StockAvailable;
        }
    }
}
