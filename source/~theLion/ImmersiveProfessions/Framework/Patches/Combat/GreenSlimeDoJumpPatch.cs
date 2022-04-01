/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeDoJumpPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeDoJumpPatch()
    {
        //Original = RequireMethod<GreenSlime>("doJump");
    }

    #region harmony patches

    /// <summary>Patch to detect jumping Slimes.</summary>
    [HarmonyPrefix]
    private static bool GreenSlimeDoJumpPrefix(GreenSlime __instance)
    {
        __instance.WriteData("Jumping", 200.ToString());
        return true; // run original logic
    }

    #endregion harmony patches
}