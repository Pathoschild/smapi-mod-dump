/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;

namespace PamTries.HarmonyPatches;

/// <summary>
/// Class that holds patches against NPC so Pam can fish.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class NPCPatches
{
    /// <summary>
    /// Set Pam's sprite to fish.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation_description.</param>
    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch("startRouteBehavior")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention set by Harmony.")]
    private static void StartFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals("pam_fish", StringComparison.OrdinalIgnoreCase))
            {
                __instance.extendSourceRect(0, 32);
                __instance.Sprite.tempSpriteHeight = 64;
                __instance.drawOffset.Value = new Vector2(0f, 96f);
                __instance.Sprite.ignoreSourceRectUpdates = false;
                if (Utility.isOnScreen(Utility.Vector2ToPoint(__instance.Position), 64, __instance.currentLocation))
                {
                    __instance.currentLocation.playSoundAt("slosh", __instance.getTileLocation());
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to adjust startRouteBehavior for Pam\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Reset Pam's fishing sprite when done fishing.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation_description.</param>
    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch("finishRouteBehavior")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention set by Harmony.")]
    private static void EndFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals("pam_fish", StringComparison.OrdinalIgnoreCase))
            {
                __instance.reloadSprite();
                __instance.Sprite.SpriteWidth = 16;
                __instance.Sprite.SpriteHeight = 32;
                __instance.Sprite.UpdateSourceRect();
                __instance.drawOffset.Value = Vector2.Zero;
                __instance.Halt();
                __instance.movementPause = 1;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to adjust finishRouteBehavior for Pam\n{ex}", LogLevel.Error);
        }
    }
}