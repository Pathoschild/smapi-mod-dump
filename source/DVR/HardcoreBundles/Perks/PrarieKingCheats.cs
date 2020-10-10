/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;

namespace HardcoreBundles.Perks
{
    public class PrarieKingCheats :PerkBase
    {
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var abi = Game1.currentMinigame;
            if (abi != null && abi.GetType().Name == "AbigailGame")
            {
                var fi = abi.GetType().GetField("playerInvincibleTimer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                fi.SetValue(abi, 99);
                Helper.Reflection.GetField<bool>(abi, "spreadPistol").SetValue(true);
                var dict = Helper.Reflection.GetField<Dictionary<int, int>>(abi, "activePowerups").GetValue();
                dict[2] = 100;
                var wt = abi.GetType().GetField("waveTimer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var wtime = (int)wt.GetValue(abi);
                if (wtime > 5000)
                {
                    wt.SetValue(abi, 4900);
                }
                Helper.Reflection.GetField<int>(abi, "coins").SetValue(100);
            }
        }
    }
}
