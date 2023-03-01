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

using IdentifiableCombinedRings.DataModels;
using IdentifiableCombinedRings.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewValley.Objects;

namespace IdentifiableCombinedRings.HarmonyPatches;

/// <summary>
/// Holds patches on combined rings.
/// </summary>
[HarmonyPatch(typeof(CombinedRing))]
internal class CombinedRingPatcher
{
    /// <inheritdoc cref="CombinedRing.drawInMenu(SpriteBatch, Vector2, float, float, float, StackDrawType, Color, bool)"/>
    /// <param name="__instance">Combined ring to check.</param>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CombinedRing.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    public static bool PrefixGetDisplayName(
        CombinedRing __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        if (!ModEntry.Config.OverrideCombinedRing)
        {
            return true;
        }

        try
        {
            NetList<Ring, NetRef<Ring>> combinedRings = __instance.combinedRings;
            if (combinedRings.Count <= 1 || combinedRings.Count > 2 || combinedRings[0] is CombinedRing || combinedRings[1] is CombinedRing)
            {
                return true;
            }

            int first = combinedRings[0].ParentSheetIndex;
            int second = combinedRings[1].ParentSheetIndex;

            RingPair pair = first > second ? new(second, first) : new(first, second);

            if (AssetManager.GetOverrideTexture(pair) is Texture2D texture)
            {
                spriteBatch.Draw(
                    texture,
                    location + (new Vector2(32f, 32f) * scaleSize),
                    new Rectangle(0, 0, 16, 16),
                    color * transparency,
                    0f,
                    new Vector2(8f, 8f) * scaleSize,
                    scaleSize * 4f,
                    SpriteEffects.None,
                    layerDepth);
                return false;
            }

            combinedRings[0].drawInMenu(spriteBatch, location + new Vector2(8f, 0f), scaleSize * .8f, transparency, layerDepth, drawStackNumber, color, drawShadow);
            combinedRings[1].drawInMenu(spriteBatch, location + new Vector2(-16f, 12f), scaleSize * .8f, transparency, layerDepth, drawStackNumber, color, drawShadow);

            return false;
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Failed while overriding drawing for combined ring:\n\n{ex}", LogLevel.Error);
        }

        return true;
    }
}
