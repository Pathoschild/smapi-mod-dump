using System;
using System.Collections.Generic;
using StardewValley;
using EconomyMod.Helpers;
using System.Linq;
using Newtonsoft.Json;

namespace EconomyMod.Model
{
    public class SaveState
    {
        public uint ReferenceDaysPlayed = 0;
        public bool CalculatedUsableSoil = false;
        public byte PostPoneDaysLeftDefault = 3;
        public byte PostPoneDaysLeft = 3;

        public int UsableSoil;
        internal TaxDetailed Detailed = new TaxDetailed();
        public bool HasPendingTax => ScheduledTax.Any(HasPendingTaxDelegate);
        public int? Day { get; set; }
        public List<TaxSchedule> ScheduledTax { get; set; }
        [JsonIgnore]
        public Func<TaxSchedule, bool> HasPendingTaxDelegate = c => !c.Paid && c.DayCount <= Game1.stats.DaysPlayed;

        [JsonIgnore]
        public IEnumerable<TaxSchedule> AllTaxScheduled => ScheduledTax.Where(HasPendingTaxDelegate).OrderBy(c => c.DayCount);

        [JsonIgnore]
        public IEnumerable<TaxSchedule> AllUnpaidTaxScheduled => ScheduledTax.Where(c => !c.Paid).OrderBy(c => c.DayCount);
        [JsonIgnore]
        public int PendingTaxAmount => AllTaxScheduled.Sum(c => c.Sum);
        internal void CalculatedNextTax()
        {

            var date = Convert.ToInt32(Game1.stats.DaysPlayed)
                    .ToWorldDate();



            var scheduledTaxCount = 0;
            if (Util.Config.TaxAfterFirstYear && date.DaysCount <= 112)
            {
                date.AddDays(112 - date.DaysCount);
            }

            switch (Util.Config.TaxPaymentType)
            {
                case TaxPaymentType.Daily:
                    scheduledTaxCount = date.DaysLeftToEndOfMonth;
                    for (int i = 0; i < scheduledTaxCount; i++)
                    {
                        if (i > 0) date.AddDays(1);
                        var tax = this.ScheduledTax.FirstOrDefault(c => date.DaysCount == Game1.stats.DaysPlayed);
                        if (tax == null)
                            this.ScheduledTax.Add(new TaxSchedule(date, Detailed));
                    }
                    break;
                case TaxPaymentType.Weekly:
                    scheduledTaxCount = date.DaysLeftToEndOfMonth / 7;
                    for (int i = 0; i < scheduledTaxCount; i++)
                    {
                        if (i > 0)
                            date.AddDays(7);
                        else
                        {
                            date.Next(Util.Config.DayOfPaymentWeekly);
                        }
                        this.ScheduledTax.Add(new TaxSchedule(date, Detailed));
                    }

                    break;
                case TaxPaymentType.Montly:
                    break;
                default:
                    break;
            }

        }
        //public Season season { get; set; }
    }
}
