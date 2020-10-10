using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static EconomyMod.Helpers.Helper;

namespace EconomyMod.Model
{
    public class TaxSchedule
    {
        public TaxDetailed Detailed;

        public TaxSchedule() {
        }
        public TaxSchedule(CustomWorldDate date, TaxDetailed detailed)
        {
            this.Detailed = detailed;
            Identifier = Guid.NewGuid();
            DayCount = date.DaysCount;
        }

        [JsonIgnore]
        public int Sum => this.Detailed.CalculateSum(DayCount);

        public Guid Identifier { get; set; }
        public bool Paid { get; set; }
        public int DayCount { get; set; }

    }
}
