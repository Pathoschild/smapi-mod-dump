using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGeodes.Overrides
{
    public class WCanAccessor
    {
        public static List<Vector2> tilesAffected(WateringCan wcan, Vector2 loc, int power, Farmer who)
        {
            return Mod.instance.Helper.Reflection.GetMethod(wcan, "tilesAffected").Invoke<List<Vector2>>(loc, power, who);
        }
    }

    public static class WateringCanStaminaHook
    {
        public static void Prefix(WateringCan __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            List<Vector2> source = WCanAccessor.tilesAffected( __instance, new Vector2((float)(x / 64), (float)(y / 64)), power, who);
#pragma warning disable AvoidNetField
#pragma warning disable AvoidImplicitNetFieldCast
            if (location.doesTileHaveProperty(x / 64, y / 64, "Water", "Back") != null || location.doesTileHaveProperty(x / 64, y / 64, "WaterSource", "Back") != null || location is BuildableGameLocation && (location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()) != null && ((location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()).buildingType.Equals((object)"Well") && (int)((NetFieldBase<int, NetInt>)(location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()).daysOfConstructionLeft) <= 0) || !(bool)((NetFieldBase<bool, NetBool>)location.isOutdoors) && location.doesTileHavePropertyNoNull(x / 64, y / 64, "Action", "Buildings").Equals("kitchen") && location.getTileIndexAt(x / 64, y / 64, "Buildings") == 172)
#pragma warning restore AvoidImplicitNetFieldCast
#pragma warning restore AvoidNetField
            {
            }
            else if (__instance.WaterLeft > 0 || who.hasWateringCanEnchantment)
            {
                float consumed = (float)(2 * (who.toolPower + 1)) - (float)who.FarmingLevel * 0.1f;
                float restored = Math.Min(who.HasAdornment(ToolType.WateringCan, Mod.Config.GEODE_LESS_STAMINA), 4) * 2;
                who.Stamina += Math.Min(consumed, restored);
            }
        }
    }

    public static class WateringCanRemoteUseHook
    {
        public static void Prefix(Hoe __instance, GameLocation location, ref int x, ref int y, int power, Farmer who)
        {
            if (who.HasAdornment(ToolType.WateringCan, Mod.Config.GEODE_REMOTE_USE) > 0)
            {
                x = (int)who.lastClick.X;
                y = (int)who.lastClick.Y;
            }
        }
    }
}
