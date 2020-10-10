/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RetroactiveStardew
{
    public class RetroSpecials
    {
        private IModHelper helper;
        private IMonitor Monitor;

        private ModConfig Config { get; }

        public RetroSpecials(IModHelper helper, IMonitor monitor, ModConfig Config)
        {
            this.helper = helper;
            this.Monitor = monitor;
            this.Config = Config;
        }

        public void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!this.Config.SpecialsCheck) return;

            this.CheckIfDarkTalismanShouldBeAdded();
        }

        private void CheckIfDarkTalismanShouldBeAdded()
        {
            if (Game1.player.hasMagicInk && !Game1.player.hasDarkTalisman)
            {
                this.Monitor.Log($"[Specials] Dark Talisman enabled.", LogLevel.Info);
                Game1.player.hasDarkTalisman = true;
            }
        }
    }
}