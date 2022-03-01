/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Handles patches on the NPC class to allow beach fishing.
/// </summary>
internal class NPCPatches
{
    private static readonly List<NPC> Fishers = new();

    /// <summary>
    /// resets the sprites of all people who went fishing.
    /// </summary>
    /// <remarks>Call at DayEnding.</remarks>
    public static void ResetAllFishers()
    {
        foreach (NPC npc in Fishers)
        {
            npc.Sprite.SpriteHeight = 32;
            npc.Sprite.SpriteWidth = 16;
            npc.Sprite.ignoreSourceRectUpdates = false;
            npc.Sprite.UpdateSourceRect();
            npc.drawOffset.Value = Vector2.Zero;
        }
        Fishers.Clear();
    }

    /// <summary>
    /// Extends sprite to allow for fishing sprites, which are 64px tall.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation key.</param>
    [HarmonyPostfix]
    [HarmonyPatch("startRouteBehavior")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention set by Harmony")]
    public static void StartFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals(__instance.Name.ToLowerInvariant() + "_beach_fish"))
            {
                __instance.extendSourceRect(0, 32);
                __instance.Sprite.tempSpriteHeight = 64;
                __instance.drawOffset.Value = new Vector2(0f, 96f);
                __instance.Sprite.ignoreSourceRectUpdates = false;
                if (Utility.isOnScreen(Utility.Vector2ToPoint(__instance.Position), 64, __instance.currentLocation))
                {
                    __instance.currentLocation.playSoundAt("slosh", __instance.getTileLocation());
                }
                Fishers.Add(__instance);
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Ran into errors in postfix for startRouteBehavior\n\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Resets sprite when NPCs are done fishing.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="__0">animation key.</param>
    [HarmonyPostfix]
    [HarmonyPatch("endRouteBehavior")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention set by Harmony")]
    public static void EndFishBehavior(NPC __instance, string __0)
    {
        try
        {
            if (__0.Equals(__instance.Name.ToLowerInvariant() + "_beach_fish"))
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
            Globals.ModMonitor.Log($"Ran into errors in postfix for startRouteBehavior\n\n{ex}", LogLevel.Error);
        }
    }
}