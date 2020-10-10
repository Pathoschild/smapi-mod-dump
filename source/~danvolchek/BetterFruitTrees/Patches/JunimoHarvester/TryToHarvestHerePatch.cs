/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterFruitTrees.Patches.JunimoHarvester
{
    /// <summary>If a harvestable fruit tree is nearby, start the harvest timer.</summary>
    internal class TryToHarvestHerePatch
    {
        public static void Postfix(StardewValley.Characters.JunimoHarvester __instance)
        {
            if (Utils.IsAdjacentReadyToHarvestFruitTree(__instance.getTileLocation(), __instance.currentLocation))
                Utils.GetJunimoHarvesterHarvestTimer(__instance).SetValue(2000);
        }
    }
}
