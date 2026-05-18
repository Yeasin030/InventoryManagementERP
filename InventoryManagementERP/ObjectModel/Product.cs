public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }

    public string StockStatus => Quantity == 0 ? "Out of Stock" : Quantity < 10 ? "Low Stock" : "In Stock";
    public Color StatusColor => Quantity == 0 ? Colors.Red : Quantity < 10 ? Colors.Orange : Colors.Green;
    public string TotalValue => $"${(Price * Quantity):F2}";
}