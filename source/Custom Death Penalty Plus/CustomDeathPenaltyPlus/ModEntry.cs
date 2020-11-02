/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace CustomDeathPenaltyPlus
{
    public class ModEntry
        : Mod
    {
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            
            this.config = this.Helper.ReadConfig<ModConfig>();

            PlayerStateSaver.SetConfig(this.config);
            AssetEditor.SetConfig(this.config);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //Use respective default values if config has invalid values
            if (config.MoneytoRestorePercentage > 1 || config.MoneytoRestorePercentage < 0 || config.MoneyLossCap < 0 || config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage <= 0 || config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
            {
                if (config.MoneytoRestorePercentage > 1 || config.MoneytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"RestoreMoneyPercentage is an invalid value, default value will be used instead... {config.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.MoneytoRestorePercentage = 0.95;
                }

                if (config.MoneyLossCap < 0)
                {
                    this.Monitor.Log("MoneyLossCap is an invalid value, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                    config.MoneyLossCap = 500;
                }

                if (config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"EnergytoRestorePercentage is an invalid value, default value will be used instead... {config.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.EnergytoRestorePercentage = 0.10;
                }

                if (config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
                {
                    this.Monitor.Log($"HealthtoRestorePercentage is an invalid value, default value will be used instead... {config.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Warn);
                    config.HealthtoRestorePercentage = 0.50;
                }
            }
            //Edit UI if items will be restored
            if (config.RestoreItems == true)
            {
                Helper.Content.AssetEditors.Add(new AssetEditor.UIFixes(Helper));
            }
            //Edit strings
            Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died each half second
            if (e.IsMultipleOf(30))
            {
                //Save money upon death and calculate amount of money to lose
                if (PlayerStateSaver.state == null && Game1.killScreen)
                {
                    PlayerStateSaver.Save();

                    //Reload asset upon death to reflect amount lost
                    Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                }
            }
            //Restore state after event ends
            else if (PlayerStateSaver.state != null && Game1.CurrentEvent == null && Game1.player.CanMove)
            {
                //Restore Player state
                PlayerStateSaver.Load();

                //Reset PlayerStateSaver
                PlayerStateSaver.state = null;
            }
        }
    }
}
