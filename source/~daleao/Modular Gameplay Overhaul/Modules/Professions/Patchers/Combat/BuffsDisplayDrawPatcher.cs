/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatcher : HarmonyPatcher
{
    private static readonly int BuffId = (Manifest.UniqueID + Profession.Brute).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffsDisplayDrawPatcher"/> class.</summary>
    internal BuffsDisplayDrawPatcher()
    {
        this.Target = this.RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Patch to draw Brute Rage buff.</summary>
    [HarmonyPostfix]
    private static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var (clickableTextureComponent, buff) = ___buffs.FirstOrDefault(p => p.Value.which == BuffId);
        if ((clickableTextureComponent, buff) == default)
        {
            return;
        }

        var counter = ProfessionsModule.State.BruteRageCounter;
        b.DrawString(
            Game1.tinyFont,
            counter.ToString(),
            new Vector2(
                clickableTextureComponent.bounds.Right - (counter >= 10 ? 16 : 8),
                clickableTextureComponent.bounds.Bottom - 24),
            Color.White);
    }
}
