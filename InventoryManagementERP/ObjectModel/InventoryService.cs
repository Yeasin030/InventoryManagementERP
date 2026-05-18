public class InventoryService
{
    private List<Product> products;

    public InventoryService()
    {
        products = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Solid Shirt", SKU = "SS001", Category = "Shirt", Price = 999.99m, Quantity = 15, LastUpdated = DateTime.Now },
            new Product { Id = Guid.NewGuid(), Name = "Jesn Pant", SKU = "JP001", Category = "Pant", Price = 29.99m, Quantity = 5, LastUpdated = DateTime.Now },
            new Product { Id = Guid.NewGuid(), Name = "Tuill Pant", SKU = "TP001", Category = "Pant", Price = 79.99m, Quantity = 0, LastUpdated = DateTime.Now },
            new Product { Id = Guid.NewGuid(), Name = "Drop Sholder", SKU = "DS001", Category = "T-Shirt", Price = 299.99m, Quantity = 8, LastUpdated = DateTime.Now },
            new Product { Id = Guid.NewGuid(), Name = "Old Money Shirt", SKU = "OMS001", Category = "Shirt", Price = 199.99m, Quantity = 12, LastUpdated = DateTime.Now }
        };
    }

    public List<Product> GetAllProducts() => products;

    public void AddProduct(Product product) => products.Add(product);

    public void UpdateProductQuantity(Guid id, int quantity)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            product.Quantity = quantity;
            product.LastUpdated = DateTime.Now;
        }
    }

    public void DeleteProduct(Guid id) => products.RemoveAll(p => p.Id == id);
}