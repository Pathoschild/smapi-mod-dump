/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawInMenuPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingDrawInMenuPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.drawInMenu), new[]
        {
            typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
            typeof(StackDrawType), typeof(Color), typeof(bool)
        });
    }

    #region harmony patches

    /// <summary>Stub for base Ring.drawInMenu</summary>
    [HarmonyReversePatch]
    internal static void RingDrawInMenuReverse(object instance, SpriteBatch spriteBatch, Vector2 location,
        float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color,
        bool drawShadow)
    {
        // its a stub so it has no initial content
        throw new NotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}