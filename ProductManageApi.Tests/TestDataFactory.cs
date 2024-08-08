using ProductManageApi.Models;

public static class TestDataFactory
{
    public static Product CreateProduct(int stock = 10)
    {
        return new Product
        {
            Name = "Test Product",
            Price = 9.99m,
            StockAvailable = stock,
            Description = "Test Description"
        };
    }
}
