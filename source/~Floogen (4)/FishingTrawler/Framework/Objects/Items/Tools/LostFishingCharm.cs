/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using StardewValley;
using StardewValley.Tools;

namespace FishingTrawler.Framework.Objects.Items.Tools
{
    public class LostFishingCharm
    {
        public static GenericTool CreateInstance()
        {
            var charm = new GenericTool();
            charm.modData[ModDataKeys.LOST_FISHING_CHARM_KEY] = true.ToString();

            return charm;
        }

        public static bool Use(GameLocation location, int x, int y, Farmer who)
        {
            if (FishingTrawler.ShouldMurphyAppear(Game1.getLocationFromName("IslandSouthEast")))
            {
                who.currentLocation.playSound("cavedrip");
                Game1.warpFarmer("IslandSouthEast", 10, 40, 2);
            }
            else if (FishingTrawler.ShouldMurphyAppear(Game1.getLocationFromName("Beach")))
            {
                who.currentLocation.playSound("cavedrip");
                Game1.warpFarmer("Beach", 87, 39, 2);
            }
            else
            {
                Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.lost_fishing_charm.no_murphy"));
            }

            who.forceCanMove();
            return false;
        }

        public static bool IsValid(Tool tool)
        {
            if (tool is not null && tool.modData.ContainsKey(ModDataKeys.LOST_FISHING_CHARM_KEY))
            {
                return true;
            }

            return false;
        }
    }
}
