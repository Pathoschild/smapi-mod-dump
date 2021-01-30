/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using SpaceCore.Events;
using StardewValley;
using xTile.Dimensions;

namespace SpaceCore.Overrides
{
    public class ActionHook
    {
        public static bool Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            return !SpaceEvents.InvokeActionActivated(who, action, tileLocation);
        }
    }

    public class TouchActionHook
    {
        public static bool Prefix(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            return !SpaceEvents.InvokeTouchActionActivated(Game1.player, fullActionString, new Location(0,0));
        }
    }

    public static class ExplodeHook
    {
        public static void Postfix(GameLocation __instance, Vector2 tileLocation, int radius, Farmer who)
        {
            SpaceEvents.InvokeBombExploded(who, tileLocation, radius);
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof( GameLocation.updateEvenIfFarmerIsntHere ) ) ]
    public static class UpdateEvenWithoutFarmerHook
    {
        public static void Postfix( GameLocation __instance, GameTime time )
        {
            // TODO: Optimize, maybe config file too?
            __instance.terrainFeatures.Values.DoIf( ( tf ) => tf is IUpdateEvenWithoutFarmer, ( tf ) => ( tf as IUpdateEvenWithoutFarmer ).UpdateEvenWithoutFarmer( __instance, time ) );
            __instance.Objects.Values.DoIf( ( o ) => o is IUpdateEvenWithoutFarmer, ( o ) => ( o as IUpdateEvenWithoutFarmer ).UpdateEvenWithoutFarmer( __instance, time ) );
        }
    }
}
