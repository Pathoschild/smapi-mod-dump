/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;


namespace CustomTokens
{
    public class LocationTokens
    {

        internal void UpdateLocationTokens(IMonitor monitor, PerScreen<PlayerData> data)
        {
            // Get current location as a MineShaft
            var mineShaft = Game1.currentLocation as MineShaft;
            // Get current location as a VolcanoDungeon
            var VolcanoShaft = Game1.currentLocation as VolcanoDungeon;

            // Test to see if current location is a MineShaft
            if (!(mineShaft is null))
            {
                // Yes, update tracker with new data

                // Display trace information in SMAPI log
                if (mineShaft.mineLevel < 121)
                {
                    monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel} of the mine.");
                }
                else if (mineShaft.mineLevel == 77377)
                {
                    monitor.Log($"{Game1.player.Name} is in the Quarry Mine.");
                }
                else
                {
                    monitor.Log($"{Game1.player.Name} on level {mineShaft.mineLevel} (level {mineShaft.mineLevel - 120} of the Skull Cavern).");
                }

                // Update trackers
                data.Value.CurrentMineLevel = mineShaft.mineLevel;

                data.Value.DeepestMineLevel = Game1.player.deepestMineLevel;
                monitor.Log($"Deepest mine level reached by {Game1.player.Name} is {data.Value.DeepestMineLevel}");
            }

            else
            {
                // No, does the tracker reflect this?
                if (data.Value.CurrentMineLevel > 0)
                {
                    // No, reset mine level tracker
                    data.Value.CurrentMineLevel = 0;
                    monitor.Log($"Minelevel tracker reset");
                }
            }

            // Test to see if current location is a Volcano Floor
            if (!(VolcanoShaft is null))
            {
                // Yes, update tracker with new data

                // Display trace information in SMAPI log
                if (VolcanoShaft.level.Value != 5)
                {
                    monitor.Log($"{Game1.player.Name} is on volcano floor {VolcanoShaft.level}.");
                }
                else
                {
                    monitor.Log($"{Game1.player.Name} is at the volcano dwarf shop. (Buy something?)");
                }

                // Update tracker
                data.Value.CurrentVolcanoFloor = VolcanoShaft.level.Value;
                if (VolcanoShaft.level.Value > data.Value.DeepestVolcanoFloor)
                {
                    data.Value.DeepestVolcanoFloor = VolcanoShaft.level.Value;
                }
            }

            else
            {
                // No, does the tracker reflect this?
                if (data.Value.CurrentVolcanoFloor > 0)
                {
                    // No, reset mine level tracker
                    data.Value.CurrentVolcanoFloor = 0;
                    monitor.Log($"VolcanoFloor tracker reset");
                }
            }
        }
    }
}
