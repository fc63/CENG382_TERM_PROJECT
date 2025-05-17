using System.Collections.Generic;
using System.Threading.Tasks;
using CENG382_TERM_PROJECT.Models;
using CENG382_TERM_PROJECT.Data;

namespace CENG382_TERM_PROJECT.Validation
{
    public class ReservationValidator
    {
        private readonly IEnumerable<IConflictRule> _rules;

        public ReservationValidator(IEnumerable<IConflictRule> rules)
        {
            _rules = rules;
        }

        public async Task<(bool IsValid, string Message)> ValidateAsync(RecurringReservation reservation)
        {
            foreach (var rule in _rules)
            {
                var (hasConflict, message) = await rule.CheckConflictAsync(reservation);
                if (hasConflict)
                    return (false, message);
            }

            return (true, null);
        }
    }
}
