/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

#pragma warning disable SA1611
namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Runtime.CompilerServices;
using System.Text;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemDrawTooltipPatcher"/> class.</summary>
    internal ItemDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.drawTooltip));
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Item.drawTooltip"/>.</summary>
    /// <remarks>Required by <see cref="Tool.drawTooltip"/> prefix.</remarks>
    [HarmonyReversePatch]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ItemDrawTooltipReverse(
        object instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}
#pragma warning restore SA1611
