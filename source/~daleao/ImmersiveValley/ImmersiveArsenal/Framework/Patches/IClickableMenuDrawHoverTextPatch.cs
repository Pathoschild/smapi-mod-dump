/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal IClickableMenuDrawHoverTextPatch()
    {
        Target = RequireMethod<IClickableMenu>(nameof(IClickableMenu.drawHoverText),
            new[]
            {
                typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int), typeof(int),
                typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int),
                typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>)
            });
    }

    #region harmony patches

    /// <summary>Compensate Slingshot enchantment effects in tooltip.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextPrefix(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: CompensateHeight(hoveredItem, ref height);
        /// Before: drawTextureBox( ... );

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)9), // arg 9 = Item hoveredItem
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .Advance(2)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, typeof(IClickableMenuDrawHoverTextPatch).RequireMethod(nameof(CompensateHeight))),
                    new CodeInstruction(OpCodes.Stloc_2)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed to compensate Slingshot tooltip height.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CompensateHeight(Item hoveredItem, int height)
    {
        if (hoveredItem is not Slingshot slingshot) return height;

        if (slingshot.GetTotalForgeLevels() > 0) height -= 12;
        if (slingshot.GetTotalForgeLevels() > 1) height -= 48;
        if (slingshot.GetTotalForgeLevels() > 2) height -= 48;
        return height;
    }

    #endregion injected subroutines
}