using Northwind.Console.Client;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var baseUrl = "https://localhost:7291/api/northwind";
        using var httpClient = new HttpClient();

        var categories = await CategoryFetcher.FetchCategories(httpClient, baseUrl);

        CategoryFetcher.DisplayCategories(categories);


        var products = await ProductFetcher.FetchProducts(httpClient, baseUrl);

        ProductFetcher.DisplayProducts(products);
    }
}