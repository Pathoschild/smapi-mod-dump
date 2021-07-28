/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewValley;

namespace AnotherHungerMod.Framework
{
    /// <summary>Encapsulates reading and writing values in players' <see cref="Character.modData"/> field.</summary>
    internal static class ModDataManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>The prefix added to mod data keys.</summary>
        private const string Prefix = "spacechase0.AnotherHungerMod";

        /// <summary>The data key for the player's current fullness points.</summary>
        private const string FullnessKey = ModDataManager.Prefix + "/Fullness";

        /// <summary>The data key for whether the player fed their spouse.</summary>
        private const string FedSpouseKey = ModDataManager.Prefix + "/FedSpouse";


        /*********
        ** Public methods
        *********/
        /// <summary>Get a player's current fullness points.</summary>
        /// <param name="player">The player to check.</param>
        public static float GetFullness(Farmer player)
        {
            return player.modData.GetFloat(ModDataManager.FullnessKey, min: 0, @default: Mod.Config.MaxFullness);
        }

        /// <summary>Set a player's current fullness points.</summary>
        /// <param name="player">The player to update.</param>
        /// <param name="value">The value to set.</param>
        public static void SetFullness(Farmer player, float value)
        {
            player.modData.SetFloat(ModDataManager.FullnessKey, value, min: 0, max: Mod.Config.MaxFullness, @default: Mod.Config.MaxFullness);
        }

        /// <summary>Get whether the player has fed their spouse.</summary>
        /// <param name="player">The player to check.</param>
        public static bool GetHasFedSpouse(Farmer player)
        {
            return player.modData.GetBool(ModDataManager.FedSpouseKey);
        }

        /// <summary>Set whether the player has fed their spouse.</summary>
        /// <param name="player">The player to update.</param>
        /// <param name="value">The value to set.</param>
        public static void SetHasFedSpouse(Farmer player, bool value)
        {
            player.modData.SetBool(ModDataManager.FedSpouseKey, value);
        }
    }
}
