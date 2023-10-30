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

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModConflict("spacechase0.MoonMisadventures")]
internal sealed class Game1DrawToolPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawToolPatcher"/> class.</summary>
    internal Game1DrawToolPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) });
    }

    #region harmony patches

    /// <summary>Replace tool texture.</summary>
    [HarmonyPrefix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void Game1DrawToolPrefix(Farmer f, ref (int, Texture2D)? __state)
    {
        var tool = f.CurrentTool;
        if (tool is null || tool.UpgradeLevel < 5 || tool is not (Axe or Hoe or Pickaxe or WateringCan))
        {
            return;
        }

        __state = (tool.UpgradeLevel, Game1.toolSpriteSheet);
        tool.upgradeLevel.Value = 4;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(Textures.RadioactiveToolsTx);
    }

    /// <summary>Restore tool texture.</summary>
    [HarmonyPostfix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void Game1DrawToolPostfix(Farmer f, (int, Texture2D)? __state)
    {
        if (!__state.HasValue)
        {
            return;
        }

        f.CurrentTool.upgradeLevel.Value = __state.Value.Item1;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(__state.Value.Item2);
    }

    #endregion harmony patches
}
