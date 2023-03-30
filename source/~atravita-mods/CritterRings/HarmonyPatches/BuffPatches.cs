/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CritterRings.Framework;
using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.Menus;

namespace CritterRings.HarmonyPatches;
/// <summary>
/// Holds a patch to give our buff a custom icon.
/// </summary>
[HarmonyPatch(typeof(Buff))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
internal static class BuffPatches
{
    [HarmonyPatch(nameof(Buff.getClickableComponents))]
    private static void Postfix(Buff __instance, List<ClickableTextureComponent> __result)
    {
        if (__instance.which == ModEntry.BunnyBuffId)
        {
            __result.Clear();
            __result.Add(new ClickableTextureComponent(
                name: string.Empty,
                bounds: Rectangle.Empty,
                label: null,
                hoverText: __instance.getDescription(__instance.which),
                texture: AssetManager.BuffTexture,
                new Rectangle(16, 0, 16, 16),
                scale: Game1.pixelZoom));
        }
    }
}
