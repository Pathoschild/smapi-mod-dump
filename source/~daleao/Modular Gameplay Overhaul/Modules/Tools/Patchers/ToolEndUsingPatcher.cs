/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Linq;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolEndUsingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolEndUsingPatcher"/> class.</summary>
    internal ToolEndUsingPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.endUsing));
    }

    #region harmony patches

    /// <summary>Do shockwave.</summary>
    [HarmonyPostfix]
    private static void ToolEndUsingPostfix(Farmer who)
    {
        var tool = who.CurrentTool;
        if (who.toolPower <= 0 || tool is not (Axe or Pickaxe))
        {
            return;
        }

        var power = who.toolPower;
#pragma warning disable CS8509
        uint radius = tool switch
#pragma warning restore CS8509
        {
            Axe => ToolsModule.Config.Axe.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => ToolsModule.Config.Pick.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1,
        };

        ToolsModule.State.Shockwaves.Add(
            new Shockwave(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }

    #endregion harmony patches
}
