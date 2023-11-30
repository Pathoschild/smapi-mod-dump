/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace HalloweenJamCSharp;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        HarmonyPatcher.ApplyPatches();

        Globals.EventHelper.GameLoop.GameLaunched += FacilityManager.InitializeVars;
        Globals.EventHelper.GameLoop.SaveLoaded += (_, _) => FacilityManager.DoLightsSetup();
        Globals.EventHelper.Player.Warped += FacilityManager.AbortLightsIfNecessary;
    }
}
