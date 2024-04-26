/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace NermNermNerm.Stardew.QuestableTractor
{
    /// <summary>
    ///   A class for polling the Master Player's ModData collection for a
    /// </summary>
    public class MasterPlayerModDataMonitor : IDisposable
    {
        private readonly IModHelper helper;
        private readonly string key;
        private string? valueAtLastCheck;
        private readonly Action onChange;

        public MasterPlayerModDataMonitor(IModHelper helper, string key, Action onChanged)
        {
            this.helper = helper;
            this.key = key;
            this.onChange = onChanged;

            if (Game1.MasterPlayer is null || Game1.player is null || Game1.IsMasterGame)
            {
                this.valueAtLastCheck = null;
            }
            else
            {
                Game1.MasterPlayer.modData.TryGetValue(key, out this.valueAtLastCheck);
            }

            this.helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_OneSecondUpdateTicked(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.MasterPlayer is null || Game1.player is null || Game1.IsMasterGame)
            {
                this.valueAtLastCheck = null;
            }
            else
            {
                Game1.MasterPlayer.modData.TryGetValue(this.key, out string? currentValue);
                if (currentValue != this.valueAtLastCheck)
                {
                    this.valueAtLastCheck = currentValue;
                    this.onChange();
                }
            }
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.OneSecondUpdateTicked -= this.GameLoop_OneSecondUpdateTicked;
        }
    }
}
