using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CENG382_TERM_PROJECT.Services
{
    public class HolidayInfo
    {
        public DateTime Date { get; set; }
        public string LocalName { get; set; }
        public string Name { get; set; }
    }

    public interface IPublicHolidayService
    {
        Task<List<HolidayInfo>> GetHolidaysAsync(int year);
    }
}
