/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Enchantments;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace AutoHookFish
{
    public class ModEntry : Mod
    {
        public const float MILLISECONDS_TO_WAIT = 250f;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            CheckNibble();
        }

        private void CheckNibble()
        {
            if (Game1.player.CurrentTool is not StardewValley.Tools.FishingRod fishingRod)
            {
                return;
            }

            // Do nothing if the fishing rod has an auto-hook enchantment.
            if (fishingRod.hasEnchantmentOfType<AutoHookEnchantment>())
            {
                return;
            }

            // Do nothing if we are not yet nibbling or have progressed into the fishing minigame.
            if (!fishingRod.isNibbling || fishingRod.isCasting)
            {
                return;
            }

            if (fishingRod.fishingNibbleAccumulator >= MILLISECONDS_TO_WAIT)
            {
                // This code is copied from the "AutoHookEnchantment" functionality in vanilla.
                /*
                fishingRod.timePerBobberBob = 1f;
                fishingRod.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                fishingRod.DoFunction(Game1.currentLocation, (int)fishingRod.bobber.X, (int)fishingRod.bobber.Y, 1, Game1.player);
                */
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
