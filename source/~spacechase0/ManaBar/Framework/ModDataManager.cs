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

namespace ManaBar.Framework
{
    /// <summary>Encapsulates reading and writing values in players' <see cref="Character.modData"/> field.</summary>
    internal static class ModDataManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>The prefix added to mod data keys.</summary>
        private const string Prefix = "spacechase0.ManaBar";

        /// <summary>The data key for the player's current mana points.</summary>
        private const string CurrentManaKey = ModDataManager.Prefix + "/CurrentMana";

        /// <summary>The data key for the player's max mana points.</summary>
        private const string MaxManaKey = ModDataManager.Prefix + "/MaxMana";


        /*********
        ** Public methods
        *********/
        /// <summary>Get a player's current mana points.</summary>
        /// <param name="player">The player to check.</param>
        public static int GetCurrentMana(Farmer player)
        {
            return player.modData.GetInt(ModDataManager.CurrentManaKey, min: 0);
        }

        /// <summary>Set a player's current mana points.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="mana">The value to set.</param>
        public static void SetCurrentMana(Farmer player, int mana)
        {
            player.modData.SetInt(ModDataManager.CurrentManaKey, mana, min: 0);
        }

        /// <summary>Get a player's max mana points.</summary>
        /// <param name="player">The player to check.</param>
        public static int GetMaxMana(Farmer player)
        {
            return player.modData.GetInt(ModDataManager.MaxManaKey, min: 0);
        }

        /// <summary>Set a player's max mana points.</summary>
        /// <param name="player">The player to check.</param>
        /// <param name="mana">The value to set.</param>
        public static void SetMaxMana(Farmer player, int mana)
        {
            player.modData.SetInt(ModDataManager.MaxManaKey, mana, min: 0);
        }
    }
}
