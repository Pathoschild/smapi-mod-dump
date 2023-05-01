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

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcReceiveGiftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcReceiveGiftPatcher"/> class.</summary>
    internal NpcReceiveGiftPatcher()
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.receiveGift));
    }

    #region harmony patches

    /// <summary>Complete Generosity quest.</summary>
    [HarmonyPostfix]
    private static void CommunityUpgradeAcceptPostfix(SObject o, Farmer giver)
    {
        giver.Increment(DataKeys.ProvenGenerosity, o.sellToStorePrice());
    }

    #endregion harmony patches
}
