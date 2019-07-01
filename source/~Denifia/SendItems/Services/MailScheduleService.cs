using System;
using Denifia.Stardew.SendItems.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Denifia.Stardew.SendItems.Services
{
    public class MailScheduleService : IMailScheduleService
    {
        private readonly IMod _mod;
        private readonly IConfigurationService _configService;

        public MailScheduleService(IMod mod, IConfigurationService configService)
        {
            _mod = mod;
            _configService = configService;

            var events = mod.Helper.Events;
            events.GameLoop.DayStarted += OnDayStarted;
            events.GameLoop.TimeChanged += OnTimeChanged;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Deliver mail each morning
            ModEvents.RaiseOnMailCleanup(this, EventArgs.Empty);
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Deliver mail every 30 mins
            var timeString = e.NewTime.ToString();
            var correctTime = timeString.EndsWith("30") || timeString.EndsWith("00");

            if (_configService.InDebugMode() && e.NewTime != 600 && correctTime)
            {
                ModEvents.RaiseOnMailCleanup(this, EventArgs.Empty);
            }
        }
    }
}
