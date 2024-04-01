/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.Common;
using StardewModdingAPI;
using StardewValley;

namespace NeverToxic.StardewMods.YetAnotherTimeMod.Framework
{
    internal class TimeHelper(ModConfig config, IMonitor monitor)
    {
        private int _lastTimeInterval = 0;

        private int _speedPercentage = config.DefaultSpeedPercentage;

        private bool _isTimeFrozen = false;

        private int SpeedPercentage
        {
            get => this._speedPercentage;
            set
            {
                if (value is > 0 and <= 700)
                {
                    this._speedPercentage = value;
                    this.OnSpeedUpdate();
                }
            }
        }

        private bool IsTimeFrozen
        {
            get => this._isTimeFrozen;
            set
            {
                this._isTimeFrozen = value;
                this.OnFreezeUpdate();
            }
        }

        public void Update()
        {
            if (this.IsTimeFrozen)
                Game1.gameTimeInterval = 0;

            if (Game1.gameTimeInterval < this._lastTimeInterval)
                this._lastTimeInterval = 0;

            Game1.gameTimeInterval = this._lastTimeInterval + (Game1.gameTimeInterval - this._lastTimeInterval) * this.SpeedPercentage / 100;
            this._lastTimeInterval = Game1.gameTimeInterval;
        }

        private void OnSpeedUpdate()
        {
            string message = I18n.Message_TimeUpdate(Percentage: this.SpeedPercentage);
            monitor.Log(message, LogLevel.Info);
            Notifier.DisplayHudNotification(message);
        }

        private void OnFreezeUpdate()
        {
            string message = this.IsTimeFrozen ? I18n.Message_TimeFrozen() : I18n.Message_TimeUnfrozen();
            monitor.Log(message, LogLevel.Info);
            Notifier.DisplayHudNotification(message, logLevel: LogLevel.Warn);
        }

        public void IncreaseSpeed()
        {
            this.SpeedPercentage += 10;
        }

        public void DecreaseSpeed()
        {
            this.SpeedPercentage -= 10;
        }

        public void ResetSpeed()
        {
            this.SpeedPercentage = config.DefaultSpeedPercentage;
        }

        public void SetHalfSpeed()
        {
            this.SpeedPercentage = 50;
        }

        public void SetDoubleSpeed()
        {
            this.SpeedPercentage = 200;
        }

        public void ToggleFreeze()
        {
            this.IsTimeFrozen = !this.IsTimeFrozen;
        }
    }
}
