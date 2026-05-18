using System.Collections.ObjectModel;


namespace InventoryManagementERP
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Product> products;
        private ObservableCollection<Product> filteredProducts;
        private InventoryService inventoryService;

        public MainPage()
        {
            InitializeComponent();
            inventoryService = new InventoryService();
            products = new ObservableCollection<Product>(inventoryService.GetAllProducts());
            filteredProducts = new ObservableCollection<Product>(products);
            ProductsCollectionView.ItemsSource = filteredProducts;
            UpdateStats();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? "";
            filteredProducts.Clear();

            var filtered = products.Where(p =>
                p.Name.ToLower().Contains(searchText) ||
                p.SKU.ToLower().Contains(searchText) ||
                p.Category.ToLower().Contains(searchText));

            foreach (var product in filtered)
                filteredProducts.Add(product);
        }

        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Add Product", "Enter product name:");
            if (string.IsNullOrWhiteSpace(result)) return;

            var sku = await DisplayPromptAsync("Add Product", "Enter SKU:");
            if (string.IsNullOrWhiteSpace(sku)) return;

            var category = await DisplayPromptAsync("Add Product", "Enter category:");
            var priceStr = await DisplayPromptAsync("Add Product", "Enter price:", keyboard: Keyboard.Numeric);
            var quantityStr = await DisplayPromptAsync("Add Product", "Enter quantity:", keyboard: Keyboard.Numeric);

            if (decimal.TryParse(priceStr, out decimal price) && int.TryParse(quantityStr, out int quantity))
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = result,
                    SKU = sku,
                    Category = category ?? "General",
                    Price = price,
                    Quantity = quantity,
                    LastUpdated = DateTime.Now
                };

                inventoryService.AddProduct(product);
                products.Add(product);
                filteredProducts.Add(product);
                UpdateStats();
                await DisplayAlert("Success", "Product added successfully!", "OK");
            }
        }

        private async void OnProductTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Product product)
            {
                var action = await DisplayActionSheet(
                    $"{product.Name}",
                    "Cancel",
                    "Delete",
                    "Update Stock",
                    "Edit Price");

                switch (action)
                {
                    case "Update Stock":
                        await UpdateStock(product);
                        break;
                    case "Edit Price":
                        await EditPrice(product);
                        break;
                    case "Delete":
                        await DeleteProduct(product);
                        break;
                }
            }
        }

        private async Task UpdateStock(Product product)
        {
            var result = await DisplayPromptAsync(
                "Update Stock",
                $"Current: {product.Quantity}\nEnter new quantity:",
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int newQuantity))
            {
                inventoryService.UpdateProductQuantity(product.Id, newQuantity);
                product.Quantity = newQuantity;
                product.LastUpdated = DateTime.Now;
                UpdateStats();
                ProductsCollectionView.ItemsSource = null;
                ProductsCollectionView.ItemsSource = filteredProducts;
            }
        }

        private async Task EditPrice(Product product)
        {
            var result = await DisplayPromptAsync(
                "Edit Price",
                $"Current: ${product.Price:F2}\nEnter new price:",
                keyboard: Keyboard.Numeric);

            if (decimal.TryParse(result, out decimal newPrice))
            {
                product.Price = newPrice;
                product.LastUpdated = DateTime.Now;
                ProductsCollectionView.ItemsSource = null;
                ProductsCollectionView.ItemsSource = filteredProducts;
            }
        }

        private async Task DeleteProduct(Product product)
        {
            bool confirm = await DisplayAlert(
                "Confirm Delete",
                $"Delete {product.Name}?",
                "Yes",
                "No");

            if (confirm)
            {
                inventoryService.DeleteProduct(product.Id);
                products.Remove(product);
                filteredProducts.Remove(product);
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            TotalProductsLabel.Text = products.Count.ToString();
            TotalValueLabel.Text = $"${products.Sum(p => p.Price * p.Quantity):F2}";
            LowStockLabel.Text = products.Count(p => p.Quantity < 10).ToString();
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var filter = picker?.SelectedItem?.ToString();

            filteredProducts.Clear();
            IEnumerable<Product> filtered = filter switch
            {
                "Low Stock" => products.Where(p => p.Quantity < 10),
                "Out of Stock" => products.Where(p => p.Quantity == 0),
                "All Products" => products,
                _ => products.Where(p => p.Category == filter)
            };

            foreach (var product in filtered)
                filteredProducts.Add(product);
        }
    }
}
    
    
