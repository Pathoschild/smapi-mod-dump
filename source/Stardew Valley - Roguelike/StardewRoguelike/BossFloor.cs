/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewRoguelike.Bosses;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewRoguelike
{
    public static class BossFloor
    {
        /// <summary>
        /// Determines whether a specified MineShaft is a boss floor.
        /// </summary>
        /// <param name="mine">The MineShaft.</param>
        /// <returns>true if it is, false otherwise</returns>
        public static bool IsBossFloor(MineShaft mine)
        {
            return IsBossFloor(Roguelike.GetLevelFromMineshaft(mine));
        }

        /// <summary>
        /// Determines whether a level is a boss floor.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>true if it is, false otherwise</returns>
        public static bool IsBossFloor(int level)
        {
            if (DebugCommands.ForcedBossIndex != -1)
                return true;

            return Merchant.IsMerchantFloor(level + 1);
        }

        /// <summary>
        /// Gets the index (which boss) is at a specified floor.
        /// </summary>
        /// <param name="floor">The floor.</param>
        /// <returns>The index of the boss that is at that floor.</returns>
        public static int GetBossIndexForFloor(int floor)
        {
            if (DebugCommands.ForcedBossIndex != -1)
                return DebugCommands.ForcedBossIndex;

            return floor % 48 / 6;
        }

        /// <summary>
        /// Calculates the difficulty of any specified MineShaft.
        /// </summary>
        /// <param name="mine">The MineShaft.</param>
        /// <returns>The floor's difficulty.</returns>
        public static float GetLevelDifficulty(MineShaft mine)
        {
            return GetLevelDifficulty(Roguelike.GetLevelFromMineshaft(mine));
        }

        /// <summary>
        /// Calculates the difficulty of any specified floor.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The floor's difficulty.</returns>
        public static float GetLevelDifficulty(int level)
        {
            if (DebugCommands.ForcedDifficulty != 0f)
                return DebugCommands.ForcedDifficulty;

            float modifier = 1f + (Roguelike.HardMode ? 0.25f : 0f);
            return (float)Math.Round(level / (float)Roguelike.ScalingOrder[^1], 2) + modifier;
        }

        /// <summary>
        /// Spawns a boss in a specified MineShaft.
        /// The boss is instantiated with the proper difficulty.
        /// </summary>
        /// <param name="mine">The mine.</param>
        public static void SpawnBoss(MineShaft mine)
        {
            Type bossType;
            int level = Roguelike.GetLevelFromMineshaft(mine);
            int index = GetBossIndexForFloor(level);

            bossType = BossManager.mainBossTypes[index];
            float difficulty = GetLevelDifficulty(mine);

            if (bossType == typeof(TutorialSlime) && difficulty >= 2f)
                bossType = typeof(LoopedSlime);

            Monster boss = (Monster)Activator.CreateInstance(bossType, new object[] { difficulty });
            mine.characters.Add(boss);
        }

        /// <summary>
        /// Spawns a specified boss in a specified MineShaft.
        /// The boss is instantiated with the proper difficulty.
        /// </summary>
        /// <param name="mine">The mine.</param>
        public static void SpawnBoss(MineShaft mine, Type whichBoss)
        {
            float difficulty = GetLevelDifficulty(mine);

            Monster boss = (Monster)Activator.CreateInstance(whichBoss, new object[] { difficulty });
            mine.characters.Add(boss);
        }

        /// <summary>
        /// Gets the map path to load for the specified <see cref="GameLocation"/>
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The map path to load.</returns>
        public static string GetMapPath(GameLocation location)
        {
            foreach (NPC character in location.characters)
            {
                if (BossManager.mainBossTypes.Contains(character.GetType()) || BossManager.miscBossTypes.Contains(character.GetType()))
                    return ((IBossMonster)character).MapPath;
            }

            return "";
        }

        /// <summary>
        /// Gets all music tracks for a specified <see cref="GameLocation"/>
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>List of music tracks that can play.</returns>
        public static List<string> GetMusicTracks(GameLocation location)
        {
            foreach (NPC character in location.characters)
            {
                if (BossManager.mainBossTypes.Contains(character.GetType()) || BossManager.miscBossTypes.Contains(character.GetType()))
                    return ((IBossMonster)character).MusicTracks;
            }

            return new() { "none" };
        }
    }
}
