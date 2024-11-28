using Northwind.Domain.Entities;
using System.Net.Http.Json;


namespace Northwind.Console.Client
{
    public static class CategoryFetcher
    {
        public static async Task<List<Category>> FetchCategories(HttpClient httpClient, string baseUrl)
        {
            var categoriesUrl = $"{baseUrl}/categories";
            System.Console.WriteLine("Fetching categories...");
            try
            {
                var response = await httpClient.GetAsync(categoriesUrl);
                response.EnsureSuccessStatusCode();

                var categories = await response.Content.ReadFromJsonAsync<List<Category>>();

                return categories;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new List<Category>(); 
            }
        }

        public static void DisplayCategories(List<Category> categories)
        {
            if (categories.Count == 0)
            {
                System.Console.WriteLine("No categories available.");
                return;
            }

            System.Console.WriteLine("\nCategories:");
            foreach (var category in categories)
            {
                System.Console.WriteLine($"ID: {category.CategoryId}, Name: {category.CategoryName}, Description: {category.Description}");
            }
        }
    }
}
