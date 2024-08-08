// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using ProductManageApi.Models;
using ProductManageApi.Repositories;

namespace ProductManageApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _repository;

        public ProductController(ProductRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            await _repository.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            await _repository.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _repository.DeleteProductByIdAsync(id);
            return NoContent();
        }

        [HttpPut("decrement-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecrementStock(int id, int quantity)
        {
            var updatedStock = await _repository.DecrementStockAsync(id, quantity);
            if (updatedStock == null)
            {
                return NotFound("Product not found.");
            }
            if (updatedStock < quantity)
            {
                return BadRequest($"Insufficient stock. Available: {updatedStock}");
            }
            return Ok(new { StockAvailable = updatedStock });
        }


        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> AddToStock(int id, int quantity)
        {
            var updatedStock = await _repository.AddToStockAsync(id, quantity);
            if (updatedStock == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(new { StockAvailable = updatedStock });
        }
    }
}
