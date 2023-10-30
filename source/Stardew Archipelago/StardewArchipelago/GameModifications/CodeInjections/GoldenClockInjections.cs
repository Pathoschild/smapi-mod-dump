/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class GoldenClockInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public virtual bool doAction(Vector2 tileLocation, Farmer who)
        public static void DoAction_GoldenClockIncreaseTime_Postfix(Building __instance, Vector2 tileLocation, Farmer who, ref bool __result)
        {
            try
            {
                if (Game1.MasterPlayer != who || 
                    !__instance.buildingType.Value.Equals("Gold Clock") ||
                    __instance.isTilePassable(tileLocation))
                {
                    return;
                }

                Game1.performTenMinuteClockUpdate();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DoAction_GoldenClockIncreaseTime_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
