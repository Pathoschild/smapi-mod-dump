/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal class BuffsDisplayDrawPatch : BasePatch
{
    private static readonly int _buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Brute;

    /// <summary>Construct an instance.</summary>
    internal BuffsDisplayDrawPatch()
    {
        Original = RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] {typeof(SpriteBatch)});
    }

    /// <summary>Patch to draw Brute Rage buff.</summary>
    [HarmonyPostfix]
    internal static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var (key, value) = ___buffs.FirstOrDefault(p => p.Value.which == _buffId);
        if (value is null) return;

        var counter = ModEntry.PlayerState.BruteRageCounter;
        b.DrawString(Game1.tinyFont, counter.ToString(),
            new(key.bounds.Right - (counter >= 10 ? 16 : 8), key.bounds.Bottom - 24), Color.White);
    }
}