/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class InventoryMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="InventoryMenuDrawPatcher"/> class.</summary>
    internal InventoryMenuDrawPatcher()
    {
        this.Target = this.RequireMethod<InventoryMenu>(
            nameof(InventoryMenu.draw),
            new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) });
    }

    #region harmony patches

    /// <summary>Draw selectable indicator.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? InventoryMenuDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var i = 0;
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Item).RequireMethod(
                                nameof(Item.drawInMenu),
                                new[]
                                {
                                    typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float),
                                    typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool),
                                })),
                    },
                    index =>
                    {
                        helper
                            .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous, i == 0 ? 5 : 3)
                            .StripLabels(out var labels)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[i == 0 ? 9 : 12]),
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldarg_1),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(InventoryMenuDrawPatcher).RequireMethod(nameof(DrawSelector))),
                                },
                                labels)
                            .GoTo(index + 5);
                        i++;
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing tool selector in inventory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawSelector(int k, InventoryMenu instance, SpriteBatch b)
    {
        if (!instance.playerInventory || ToolsModule.State.SelectableToolByType.Count == 0)
        {
            return;
        }

        if (instance.actualInventory[k] is Tool tool &&
            ToolsModule.State.SelectableToolByType.TryGetValue(tool.GetType(), out var selectable) &&
            selectable?.Tool == tool)
        {
            instance.inventory[k].bounds.DrawBorder(ToolsModule.Config.SelectionBorderColor, b);
        }
    }

    #endregion injected subroutines
}
