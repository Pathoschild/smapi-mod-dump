/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static readonly int _buffId = (ModEntry.Manifest.UniqueID + "Energized").GetHashCode();

    /// <summary>Construct an instance.</summary>
    internal BuffsDisplayDrawPatch()
    {
        Target = RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Patch to draw Energized buff.</summary>
    [HarmonyPostfix]
    internal static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var (clickableTextureComponent, buff) = ___buffs.FirstOrDefault(p => p.Value.which == _buffId);
        if ((clickableTextureComponent, buff) == default) return;

        var counter = ModEntry.State.EnergizeStacks;
        b.DrawString(Game1.tinyFont, counter.ToString(),
            new(clickableTextureComponent.bounds.Right - (counter >= 10 ? 16 : 8), clickableTextureComponent.bounds.Bottom - 24), Color.White);
    }
}