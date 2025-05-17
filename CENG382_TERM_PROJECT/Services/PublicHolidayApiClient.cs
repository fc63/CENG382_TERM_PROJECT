using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CENG382_TERM_PROJECT.Services
{
    public class PublicHolidayApiClient
    {
        private readonly HttpClient _httpClient;

        public PublicHolidayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<(DateOnly date, string description)>> FetchHolidaysAsync(int year)
        {
            var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/TR";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json).RootElement;

            var results = new List<(DateOnly, string)>();

            foreach (var item in root.EnumerateArray())
            {
                var date = DateOnly.Parse(item.GetProperty("date").GetString());
                var description = item.GetProperty("localName").GetString();
                results.Add((date, description));
            }

            return results;
        }
    }
}
