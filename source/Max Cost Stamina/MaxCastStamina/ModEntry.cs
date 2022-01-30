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
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.CurrentTool is FishingRod rod)
            {
                newCastPower = rod.castingPower;
                newStamina = Game1.player.stamina;
                if (newCastPower > 0.99f)
                {
                    if (newCastPower == oldCastPower)
                    {
                        if (newStamina < oldStamina)
                        {
                            rod.castingPower = 0;
                            Game1.player.stamina = oldStamina;
                        }
                    }
                }
                oldCastPower = newCastPower;
                oldStamina = newStamina;
            }
        }
    }
}
