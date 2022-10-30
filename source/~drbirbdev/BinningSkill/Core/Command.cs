/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared.Command;
using StardewValley;

namespace BinningSkill
{
    [CommandClass]
    public class Command
    {

        [CommandMethod("Get a random drop from the loot table for given location, trash can, and rarity")]
        public static void GetRandomTrashDrop(string location, string whichCan, int rarity = -1)
        {
            if (rarity == -1)
            {
                rarity = BirbShared.Utilities.GetRarity(Utilities.GetBinningRarityLevels());
            }
            string dropString = BirbShared.Utilities.GetRandomDropStringFromLootTable(ModEntry.Assets.TrashTable, location, whichCan, rarity.ToString());

            Game1.player.addItemByMenuIfNecessary(BirbShared.Utilities.ParseDropString(dropString, ModEntry.JsonAssets, ModEntry.DynamicGameAssets));
        }

        [CommandMethod("Get a particular drop for a given item id in the trashdrops json format")]
        public static void GetTrashDrop(string id)
        {
            Game1.player.addItemByMenuIfNecessary(BirbShared.Utilities.ParseDropString(id, ModEntry.JsonAssets, ModEntry.DynamicGameAssets));
        }
    }
}
