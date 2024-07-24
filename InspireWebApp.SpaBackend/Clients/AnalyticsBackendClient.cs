using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InspireWebApp.SpaBackend.Clients;

public class AnalyticsBackendClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AnalyticsBackendClient()
    {
        _baseUrl = "http://127.0.0.1:5000";
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public  Task<IReadOnlyList<CustomerSegment>> GetSegmentDetailsAsync()
    {
        return GetAsync<IReadOnlyList<CustomerSegment>>("segment-details");
    }
    
    public  Task<List<SalesForecast>> GetSalesForecasting()
    {
        return GetAsync<List<SalesForecast>>("/sales/forecasting");
    }

    // Method to GET data from the API
    private async Task<T> GetAsync<T>(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        string responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    // Method to POST data to the API
    private async Task<T> PostAsync<T>(string endpoint, object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        string responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    // Method to PUT data to the API
    private async Task<T> PutAsync<T>(string endpoint, object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        string responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    // Method to DELETE data from the API
    private async Task DeleteAsync(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }
    
    public struct CustomerSegment
    {
        public string Segment { get; set; }

        public string Description { get; set; }

        public string TypicalActions { get; set; }
    }
    
    public struct SalesForecast
    {
        public string YearMonth { get; set; }

        public decimal TotalSales { get; set; }
    }
}
