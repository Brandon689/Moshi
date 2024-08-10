using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Moshi.MyAnimeList.Models;

namespace AnimeTracker
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7243";

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<List<MoshiAnime>> GetAllAnimeAsync()
        {
            var response = await _httpClient.GetAsync("/anime");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MoshiAnime>>(content);
        }

        public async Task<List<MoshiAnime>> SearchAnimeAsync(string title)
        {
            var response = await _httpClient.GetAsync($"/anime/search?title={Uri.EscapeDataString(title)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MoshiAnime>>(content);
        }

        public async Task<MoshiAnime> GetAnimeByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/anime/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MoshiAnime>(content);
        }

        public async Task<MoshiAnime> AddAnimeAsync(MoshiAnime anime)
        {
            var json = JsonSerializer.Serialize(anime);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/anime", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MoshiAnime>(responseContent);
        }

        public async Task<MoshiAnime> UpdateAnimeAsync(int id, MoshiAnime anime)
        {
            var json = JsonSerializer.Serialize(anime);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/anime/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MoshiAnime>(responseContent);
        }

        public async Task DeleteAnimeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/anime/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
