/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using GingerIslandStart.Additions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace GingerIslandStart.Events;

public class Day
{
    private static Point Start => ModEntry.StartingPoint;
    private static string NameInData => ModEntry.NameInData;
    private static string RecoveryKey => ModEntry.RecoveryKey;
    private static double Difficulty => ModEntry.GeneralDifficultyMultiplier;
    private static IModHelper Help => ModEntry.Help;


    internal static void OnStart(object sender, DayStartedEventArgs e)
    {
        //if not an island save
        if (!Game1.player.modData.ContainsKey(NameInData))
            return;

        if(!Game1.player.mailReceived.Contains(ModEntry.GiftWarpId))
            IslandChanges.ChangeGiftLocation();

        TryAddQuest();
        
    	var recoveryItem = Help.Data.ReadSaveData<ItemSaveData>(RecoveryKey);
        if (recoveryItem != null)
        {
            //depending on farmhouse unlock: set variables to tent OR house location
            var hasHouse = Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse");
            var locationName = hasHouse ? "IslandWest" : "IslandSouth";
            var position = hasHouse ? new Vector2(79, 40) : new Vector2(Start.X - 1, Start.Y);
            var chest = new Chest(new List<Item>(), position, true);
            chest.Items.Add(recoveryItem.GetItem());
            
            //place it, and remove recovery data from save to avoid doing every day
            var where = Utility.fuzzyLocationSearch(locationName);
            where.Objects.Add(position, chest);
            Help.Data.WriteSaveData<ItemSaveData>(RecoveryKey, null);
        }

        if (!Game1.player.canUnderstandDwarves && !Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
        {
            /* multiple ways to unlock dwarves
             * find a diamond OR prismatic shard
             * find 3,5,9 gems (depends on difficulty)
             * total ores is 10/20/40
             * crush 15/30/60 rocks
             */
            var totalOres = Game1.player.stats.CopperFound + Game1.player.stats.IronFound + Game1.player.stats.GoldFound + Game1.player.stats.IridiumFound;
            
            var foundRareGem = Game1.player.stats.DiamondsFound > 0 || Game1.player.stats.PrismaticShardsFound > 0;
            var foundGems = Game1.player.stats.OtherPreciousGemsFound > 4 * Difficulty;
            var enoughOres = totalOres >= 20 * Difficulty;
            var crushedRocks = Game1.player.stats.RocksCrushed >= 30 * Difficulty;
            
            if (foundRareGem || foundGems || enoughOres || crushedRocks)
                Game1.player.canUnderstandDwarves = true;
        }
        
        if (!ModEntry.NeedsWarp)
            return;

        Location.WarpToIsland();
    }

    internal static void TryAddQuest()
    {
        var farmersWithoutQuest = Game1.getAllFarmers().Where(f => f.hasQuest($"{ModEntry.Id}_StarterQuest") == false && f.mailReceived.Contains(ModEntry.Id)).ToList();
        foreach (var farmer in farmersWithoutQuest)
            farmer.addQuest($"{ModEntry.Id}_StarterQuest");
    }

    internal static void OnEnd(object sender, DayEndingEventArgs e)
    {
        //if not an island save
        if (!Game1.player.modData.ContainsKey(NameInData))
            return;
        
        //if has boat OR island house
        if(Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
            return;
        
        //if has boat OR island house
        if(!Game1.player.hasOrWillReceiveMail("Island_Resort"))
            return;
        
        var where = Utility.fuzzyLocationSearch("IslandSouth");
        ((StardewValley.Locations.IslandSouth)where).resortOpenToday.Value = false;
    }
}