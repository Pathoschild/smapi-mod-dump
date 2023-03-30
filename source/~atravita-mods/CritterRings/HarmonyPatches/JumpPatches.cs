/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CritterRings;

using HarmonyLib;

using StardewValley.Tools;

/// <summary>
/// Patches to make sure the player doesn't move in certain times.
/// </summary>
[HarmonyPatch]
internal static class JumpPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.leftClick))]
    private static bool PrefixSwordSwing(Farmer who)
    {
        return ModEntry.CurrentJumper?.IsValid(out Farmer? farmer) != true || !ReferenceEquals(who, farmer);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game1), nameof(Game1.pressUseToolButton))]
    private static bool PrefixUseTool()
    {
        return ModEntry.CurrentJumper?.IsValid(out Farmer? farmer) != true;
    }
}