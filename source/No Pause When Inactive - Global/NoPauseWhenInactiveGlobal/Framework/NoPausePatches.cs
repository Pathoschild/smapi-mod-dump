/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thakyZ/StardewValleyMods
**
*************************************************/

using HarmonyLib;

using StardewValley;
using StardewValley.SDKs;

namespace NoPauseWhenInactiveGlobal.Framework;

/// <summary>
/// Patch the <see cref="InstanceGame.IsActive"/> property getter method.
/// </summary>
[HarmonyPatch(typeof(InstanceGame), "get_" + nameof(InstanceGame.IsActive))]
[HarmonyDebug]
public static class InstanceGame_IsActive_Patch
{
    /// <summary>
    /// A generic Prefix method.
    /// </summary>
    /// <param name="__instance">The instance of <see cref="InstanceGame"/></param>
    /// <param name="__result">The resulting boolean value of the getter.</param>
    /// <returns>A boolean whether or not the game is active.</returns>
    public static bool Prefix(InstanceGame __instance, ref bool __result)
    {
        if (ModEntry.Config.DisableGamePause && !ModEntry.IsSaveLoaded)
        {
            __result = true;
            return false;
        }
        return true;
    }
}

/// <summary>
/// Patch the <see cref="InstanceGame.IsActiveNoOverlay"/> property getter method.
/// </summary>
[HarmonyPatch(typeof(Game1), "get_" + nameof(Game1.IsActiveNoOverlay))]
[HarmonyDebug]
public static class Game1_IsActiveNoOverlay_Patch
{
    /// <summary>
    /// A generic Prefix method.
    /// </summary>
    /// <param name="__instance">The instance of <see cref="Game1"/></param>
    /// <param name="__result">The resulting boolean value of the getter.</param>
    /// <returns>A boolean whether or not the game is active.</returns>
    public static bool Prefix(Game1 __instance, ref bool __result)
    {
        // Program.sdk is an internal labeled function so we must use HarmonyLib.Traverse to get it's value.
        var Program_sdk = Traverse.Create(typeof(Program)).Property("sdk").GetValue<SDKHelper>();
        if (Program_sdk != null && ModEntry.Config.DisableGamePause && !ModEntry.IsSaveLoaded && !Program_sdk.HasOverlay)
        {
            __result = true;
            return false;
        }
        return true;
    }
}
