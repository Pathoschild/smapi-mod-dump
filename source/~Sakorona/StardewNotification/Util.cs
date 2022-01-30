/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace StardewNotification
{
    public static class Util
    {
        private const int MUSHROOM_CAVE = 2;

        public static void ShowMessage(string msg)
        {
            var hudmsg = new HUDMessage(msg, Color.SeaGreen, StardewNotification.Config.NotificationDuration , true)
            {
                whatType = 2
            };
            Game1.addHUDMessage(hudmsg);
        }

        public static void ShowHarvestableMessage(ITranslationHelper Trans, KeyValuePair<string, Pair<StardewValley.Object, int>> pair)
        {
            var item = CopyObject(pair.Value.First);
            item.name = Trans.Get("readyHarvest", new { cropName = pair.Key }); 
            item.bigCraftable.Value = pair.Value.First.bigCraftable.Value;
            Game1.addHUDMessage(new HUDMessage(pair.Key, pair.Value.Second, true, Color.OrangeRed, item));
        }
              

        public static Object CopyObject(Object source)
        {
			Object dst;
            dst = new Object(source.ParentSheetIndex, source.Stack, false, source.Price, source.Quality);
            if (Game1.player.caveChoice.Value == MUSHROOM_CAVE)
            {
                dst.bigCraftable.Value = source.bigCraftable.Value;
                dst.TileLocation = source.TileLocation;
            }
            dst.Type = source.Type;
            return dst;
        }
    }
}
