/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-MaxCastStamina
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace MaxCastStamina
{
    public class ModEntry : Mod
    {
        float oldCastPower;
        float newCastPower;

        float oldStamina;
        float newStamina;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {   
            // Only check if the player is holding a fishing rod
            if (Game1.player.CurrentTool is FishingRod rod)
            {
                newCastPower = rod.castingPower;
                newStamina = Game1.player.stamina;
                // Check for max cast
                if (newCastPower > 0.99f)
                {
                    // Make sure the player didn't just go past the max cast without stopping on it
                    if (newCastPower == oldCastPower)
                    {
                        if (newStamina < oldStamina)
                        {
                            // Handle edge case where casting would've made the player exhausted
                            if (newStamina < 0)
                            {
                                Game1.player.exhausted.Value = false;
                                Game1.addHUDMessage(HUDMessage.ForCornerTextbox("But luckily your fishing skills saved you!"));
                            }
                            // Reset casting power for next time and give player their stamina back
                            rod.castingPower = 0;
                            Game1.player.stamina = oldStamina;
                        }
                    }
                }
                // Update stamina and casting power values for next tick
                oldCastPower = newCastPower;
                oldStamina = newStamina;
            }
        }
    }
}