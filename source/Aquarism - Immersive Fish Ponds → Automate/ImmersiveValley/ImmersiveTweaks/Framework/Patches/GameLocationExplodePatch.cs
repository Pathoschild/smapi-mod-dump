/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Classes;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationExplodePatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Explosions trigger nearby bombs.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(GameLocation __instance, Vector2 tileLocation, int radius)
    {
        if (!ModEntry.Config.ExplosionTriggeredBombs) return;

        var circle = new CircleTileGrid(tileLocation, radius * 2);
        foreach (var sprite in __instance.TemporarySprites.Where(sprite => sprite.bombRadius > 0 && circle.Tiles.Contains(sprite.Position / 64f)))
            sprite.currentNumberOfLoops = sprite.totalNumberOfLoops;
    }

    #endregion harmony patches
}