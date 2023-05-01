/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationUpdateCharactersPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationUpdateCharactersPatcher"/> class.</summary>
    internal GameLocationUpdateCharactersPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("updateCharacters");
    }

    #region harmony patches

    /// <summary>Update Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationUpdateCharactersPostfix(GameLocation __instance, GameTime time)
    {
        if (Context.IsMainPlayer && Game1.shouldTimePass())
        {
            __instance.Get_Animals().ForEach(animal => animal.update(time, __instance));
        }
    }

    #endregion harmony patches
}
