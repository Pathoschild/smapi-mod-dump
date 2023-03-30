/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class AdventurerGuildGilPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AdventurerGuildGilPatcher"/> class.</summary>
    internal AdventurerGuildGilPatcher()
    {
        this.Target = this.RequireMethod<AdventureGuild>("gil");
    }

    #region harmony patches

    /// <summary>Record Gil flag.</summary>
    [HarmonyPostfix]
    private static void AdventurerGuildGilPostfix()
    {
        var player = Game1.player;
        if (player.NumMonsterSlayerQuestsCompleted() < 5)
        {
            return;
        }

        player.WriteIfNotExists(DataKeys.ProvenValor, true.ToString());
        Virtue.Valor.CheckForCompletion(player);
    }

    #endregion harmony patches
}
