using System.Collections.Generic;

using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace StardewNotification
{
    public static class Util
    {
        public static Configuration Config { get; set; }
		public static IMonitor Monitor { get; set; }

        private const int MUSHROOM_CAVE = 2;
        private const int FRUIT_CAVE = 1;

        public static void showMessage(string msg)
        {
            var hudmsg = new HUDMessage(msg, Color.SeaGreen, 5250f, true);
            hudmsg.whatType = 2;
            Game1.addHUDMessage(hudmsg);
        }

        public static void ShowHarvestableMessage(KeyValuePair<string, Pair<StardewValley.Object, int>> pair)
        {
            var item = CopyObject(pair.Value.First);
            item.name = string.Format(Constants.READY_FOR_HARVEST, pair.Key);
            item.bigCraftable = pair.Value.First.bigCraftable;
            Game1.addHUDMessage(new HUDMessage(pair.Key, pair.Value.Second, true, Color.OrangeRed, item));
        }

        public static void ShowFarmCaveMessage(GameLocation location)
        {
            var e = location.Objects.GetEnumerator();
            e.MoveNext();
            var item = CopyObject(e.Current.Value);
            item.name = Game1.player.caveChoice == MUSHROOM_CAVE ? Constants.CAVE_MUSHROOM : Constants.CAVE_FRUIT;
            Game1.addHUDMessage(new HUDMessage(item.type, location.Objects.Count, true, Color.OrangeRed, item));
        }

        public static Object CopyObject(Object source)
        {
			Object dst;
            dst = new Object(source.ParentSheetIndex, source.Stack, false, source.Price, source.quality);
            if (Game1.player.caveChoice == MUSHROOM_CAVE)
            {
                dst.bigCraftable = source.bigCraftable;
                dst.tileLocation = source.tileLocation;
            }
            dst.type = source.Type;
            return dst;
        }
    }
}
