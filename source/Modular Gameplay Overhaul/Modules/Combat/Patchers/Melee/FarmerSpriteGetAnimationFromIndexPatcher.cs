/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerSpriteGetAnimationFromIndexPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerSpriteGetAnimationFromIndexPatcher"/> class.</summary>
    internal FarmerSpriteGetAnimationFromIndexPatcher()
    {
        this.Target = this.RequireMethod<FarmerSprite>(nameof(FarmerSprite.getAnimationFromIndex));
    }

    #region harmony patches

    /// <summary>Do weapon combo.</summary>
    [HarmonyPrefix]
    private static bool FarmerSpriteGetAnimationFromIndexPrefix(int index, FarmerSprite requester)
    {
        if (index is not (248 or 240 or 232 or 256) || !CombatModule.Config.EnableComboHits)
        {
            return true; // run original logic
        }

        try
        {
            var owner = Reflector.GetUnboundFieldGetter<FarmerSprite, Farmer>(requester, "owner")
                .Invoke(requester);
            if (!owner.IsLocalPlayer || owner.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
            {
                return true; // run original logic
            }

            var hitStep = CombatModule.State.ComboHitQueued;
            if (weapon.IsClub() && hitStep == weapon.GetFinalHitStep() - 1)
            {
                owner.QueueSmash(weapon);
            }
            else if ((int)hitStep % 2 == 0)
            {
                owner.QueueForwardSwipe(weapon);
            }
            else
            {
                owner.QueueReverseSwipe(weapon);
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
