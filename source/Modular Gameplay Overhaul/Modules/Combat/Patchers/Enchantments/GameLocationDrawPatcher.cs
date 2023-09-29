/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDrawPatcher"/> class.</summary>
    internal GameLocationDrawPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.draw));
    }

    #region harmony patches

    /// <summary>Draw Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationDrawPostfix(GameLocation __instance, SpriteBatch b)
    {
        __instance.Get_Animals().ForEach(animal => animal.draw(b));
    }

    #endregion harmony patches
}
