/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using System;
using System.Linq;
using System.Reflection;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterFindPlayerPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterFindPlayerPatch()
    {
        Target = RequireMethod<Monster>("findPlayer");
        Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer? __result)
    {
        try
        {
            var location = Game1.currentLocation;
            Farmer? target = null;
            if (__instance is GreenSlime slime && slime.get_Piper() is not null)
            {
                var aggroee = slime.GetClosestNPC(location.characters.OfType<Monster>().Where(m => !m.IsSlime()));
                if (aggroee is not null)
                {
                    var fakeFarmer = slime.get_FakeFarmer();
                    if (fakeFarmer is not null)
                    {
                        fakeFarmer.Position = aggroee.Position;
                        target = fakeFarmer;
                    }
                }
            }
            else
            {
                var taunter = __instance.get_Taunter().Get(__instance.currentLocation);
                if (taunter is not null)
                {
                    var fakeFarmer = __instance.get_FakeFarmer();
                    if (fakeFarmer is not null)
                    {
                        fakeFarmer.Position = taunter.Position;
                        target = fakeFarmer;
                    }
                }
            }

            __result = target ?? (Context.IsMultiplayer
                ? __instance.GetClosestFarmer(predicate: f => !f.get_IsFake().Value && !f.IsInAmbush())
                : Game1.player);
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