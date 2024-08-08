using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductManageApi.Data;
using ProductManageApi.Models;
using ProductManageApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManageApi.Tests
{
    [TestFixture]
    public class ProductRepositoryTests
    {
        private ProductContext _context;
        private ProductRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ProductContext(options);
            _repository = new ProductRepository(_context);

            // Seed initial data with all required properties
            _context.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", StockAvailable = 10, Price = 9.99m, Description = "Description 1" },
                new Product { Id = 2, Name = "Product 2", StockAvailable = 20, Price = 19.99m, Description = "Description 2" }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the context to free up resources
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Act
            var result = await _repository.GetAllProductsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.First(p => p.Id == 1).Id);
            Assert.AreEqual(10, result.First(p => p.Id == 1).StockAvailable);
            Assert.AreEqual(2, result.First(p => p.Id == 2).Id);
            Assert.AreEqual(20, result.First(p => p.Id == 2).StockAvailable);
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Act
            var result = await _repository.GetProductByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Product 1", result.Name);
            Assert.AreEqual(10, result.StockAvailable);
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = await _repository.GetProductByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "New Product",
                StockAvailable = 15,
                Price = 29.99m,
                Description = "New Description"
            };

            // Act
            await _repository.AddProductAsync(newProduct);

            // Assert
            var addedProduct = await _repository.GetProductByIdAsync(newProduct.Id);
            Assert.IsNotNull(addedProduct);
            Assert.AreEqual("New Product", addedProduct.Name);
            Assert.AreEqual(15, addedProduct.StockAvailable);
            Assert.AreEqual(29.99m, addedProduct.Price);
            Assert.AreEqual("New Description", addedProduct.Description);
        }
        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var productToUpdate = await _repository.GetProductByIdAsync(1);
            if (productToUpdate == null) Assert.Fail("Product not found");

            productToUpdate.Name = "Updated Product";
            productToUpdate.StockAvailable = 30;

            // Act
            await _repository.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await _repository.GetProductByIdAsync(1);
            Assert.IsNotNull(updatedProduct);
            Assert.AreEqual("Updated Product", updatedProduct.Name);
            Assert.AreEqual(30, updatedProduct.StockAvailable);
        }

        [Test]
        public async Task DeleteProductByIdAsync_ShouldRemoveProduct_WhenProductExists()
        {
            // Act
            await _repository.DeleteProductByIdAsync(1);

            // Assert
            var deletedProduct = await _repository.GetProductByIdAsync(1);
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public async Task DeleteProductByIdAsync_ShouldNotThrowException_WhenProductDoesNotExist()
        {
            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _repository.DeleteProductByIdAsync(999));
        }
        public async Task DecrementStockAsync_ShouldReturnRemainingStock_WhenStockSufficient()
        {
            // Act
            var result = await _repository.DecrementStockAsync(1, 5);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result); // 10 - 5 = 5
        }

        [Test]
        public async Task DecrementStockAsync_ShouldReturnCurrentStock_WhenStockInsufficient()
        {
            // Act
            var result = await _repository.DecrementStockAsync(1, 15);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result); // StockAvailable is not decremented
        }

        [Test]
        public async Task DecrementStockAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = await _repository.DecrementStockAsync(999, 5);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddToStockAsync_ShouldReturnUpdatedStock_WhenProductExists()
        {
            // Act
            var result = await _repository.AddToStockAsync(1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result); // 10 + 10 = 20
        }

        [Test]
        public async Task AddToStockAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = await _repository.AddToStockAsync(999, 10);

            // Assert
            Assert.IsNull(result);
        }
    }
}
