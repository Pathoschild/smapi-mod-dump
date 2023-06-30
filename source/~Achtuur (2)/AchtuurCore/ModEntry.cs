/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;

namespace AchtuurCore;

internal class ModEntry : Mod
{
    internal static ModEntry Instance;
    public override void Entry(IModHelper helper)
    {
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new WateringPatcher()
        );
        Events.EventPublisher.FinishedWateringSoil += this.OnWateredSoil;
    }

    private void OnWateredSoil(object sender, WateringFinishedArgs e)
    {
    }
}
