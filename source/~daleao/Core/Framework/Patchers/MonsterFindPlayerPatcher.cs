/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Core.Framework;
using DaLion.Core.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterFindPlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterFindPlayerPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterFindPlayerPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>("findPlayer");
    }

    #region harmony patches

    /// <summary>Patch to implement Blind status.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("DaLion.Professions", "Esca.FarmTypeManager")]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer? __result)
    {
        if (!__instance.IsBlinded() || __instance.currentLocation is not { } location ||
            (Game1.ticks + __instance.GetHashCode()) % 15 != 0)
        {
            return true; // run original logic
        }

        try
        {
            var target = __instance.Get_Target();
            if (target is not FakeFarmer)
            {
                target = new FakeFarmer();
                __instance.Set_Target(target);
            }

            var offset = Game1.random.NextBool() ? Vector2.UnitX : Vector2.UnitY;
            offset *= Game1.random.NextBool() ? 1 : -1;
            target.Position = __instance.Position + offset;
            __result = target;
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
