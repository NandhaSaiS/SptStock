using System.Net.Http.Json;
using StockManagementSystem.Shared.Models;

namespace StockManagementSystem.Client.Services;

public class StockService
{
    private readonly HttpClient _httpClient;

    public StockService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StockItem>> GetAllStockItems()
    {
        return await _httpClient.GetFromJsonAsync<List<StockItem>>("api/stock") ?? new List<StockItem>();
    }

    public async Task<List<StockItem>> SearchStockItems(string searchTerm)
    {
        return await _httpClient.GetFromJsonAsync<List<StockItem>>($"api/stock/search?term={searchTerm}") ?? new List<StockItem>();
    }

    public async Task<StockItem?> GetStockItem(int id)
    {
        return await _httpClient.GetFromJsonAsync<StockItem>($"api/stock/{id}");
    }

    public async Task<bool> CreateStockItem(StockItem stockItem)
    {
        var response = await _httpClient.PostAsJsonAsync("api/stock", stockItem);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStockItem(int id, StockItem stockItem)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/stock/{id}", stockItem);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteStockItem(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/stock/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<(bool Success, string Message)> UploadExcel(MultipartFormDataContent content)
    {
        try
        {
            var response = await _httpClient.PostAsync("api/stock/upload", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
                return (true, result?.Message ?? "Upload successful");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<byte[]?> DownloadTemplate()
    {
        try
        {
            return await _httpClient.GetByteArrayAsync("api/stock/download-template");
        }
        catch
        {
            return null;
        }
    }
}

public class UploadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Added { get; set; }
}
