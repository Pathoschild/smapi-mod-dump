/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Foraging;

[UsedImplicitly]
internal class Game1ShouldTimePassPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1ShouldTimePassPatch()
    {
        Original = RequireMethod<Game1>(nameof(Game1.shouldTimePass));
    }

    #region harmony patches

    /// <summary>Patch to freeze time during prestiged treasure hunts.</summary>
    [HarmonyPrefix]
    private static bool Game1ShouldTimePassPrefix(ref bool __result)
    {
        if ((ModState.ProspectorHunt is null || !ModState.ProspectorHunt.IsActive ||
             !Game1.player.HasPrestigedProfession("Prospector")) &&
            (ModState.ScavengerHunt is null || !ModState.ScavengerHunt.IsActive ||
             !Game1.player.HasPrestigedProfession("Scavenger"))) return true; // run original logic
        
        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}