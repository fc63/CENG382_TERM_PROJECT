using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://date.nager.at/api/v3/PublicHolidays";

        public PublicHolidayService(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<List<HolidayInfo>> GetHolidaysAsync(int year)
        {
            string cacheKey = $"holidays_{year}";
            if (_cache.TryGetValue(cacheKey, out List<HolidayInfo> cachedHolidays))
            {
                return cachedHolidays;
            }

            var response = await _httpClient.GetAsync($"{ApiUrl}/{year}/TR");
            if (!response.IsSuccessStatusCode) return new List<HolidayInfo>();

            var json = await response.Content.ReadAsStringAsync();
            var holidays = JsonConvert.DeserializeObject<List<HolidayInfo>>(json);

            _cache.Set(cacheKey, holidays, TimeSpan.FromDays(7));
            return holidays;
        }
    }
}
