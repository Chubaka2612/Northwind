using Northwind.Domain.Entities;
using System.Net.Http.Json;

namespace Northwind.Console.Client
{
    public static class ProductFetcher
    {
        public static async Task<List<Product>> FetchProducts(HttpClient httpClient, string baseUrl)
        {
            var productsUrl = $"{baseUrl}/products";
            System.Console.WriteLine("\nFetching products...");

            try
            {
                var response = await httpClient.GetAsync(productsUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var products = await response.Content.ReadFromJsonAsync<List<Product>>();

                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<Product>();
            }
        }

        public static void DisplayProducts(List<Product> products)
        {
            if (products.Count == 0)
            {
                System.Console.WriteLine("No products available.");
                return;
            }

            System.Console.WriteLine("\nProducts:");
            foreach (var product in products)
            {
                System.Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.UnitPrice:C}");
            }
        }
    }
}
