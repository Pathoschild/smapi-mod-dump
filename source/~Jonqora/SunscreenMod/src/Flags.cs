using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace SunscreenMod
{
    class Flags
    {
        /*********
        ** Accessors
        *********/
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;


        /*********
        ** Static Fields
        *********/
        /// <summary>The base key contained in all mail flags added by this mod.</summary>
        public const string FLAG_BASE = "6676SunscreenMod.";

        //Flag types used by this mod
        public const string NEW_DAY_FLAG = "NewDay";
        public const string APPLIED_ALOE_FLAG = "HasAppliedAloeToday";
        public const string SUNBURN_LEVEL_FLAG = "SunburnLevel";
        public const string NEW_BURN_LEVEL_FLAG = "NewBurnDamageLevel";

        /// <summary>List of non-dynamic mail flags used by this mod.</summary>
        static readonly string[] FlagList = new string[] {
            NEW_DAY_FLAG,
            APPLIED_ALOE_FLAG,
            $"{SUNBURN_LEVEL_FLAG}_1",
            $"{SUNBURN_LEVEL_FLAG}_2",
            $"{SUNBURN_LEVEL_FLAG}_3",
            $"{NEW_BURN_LEVEL_FLAG}_1",
            $"{NEW_BURN_LEVEL_FLAG}_2",
            $"{NEW_BURN_LEVEL_FLAG}_3"
        };


        /*********
        ** Static Methods
        *********/
        /// <summary>Checks if a flag name exists in the list of mod flags.</summary>
        public static bool IsFlag(string flagName)
        {
            return FlagList.Contains(flagName);
        }

        /// <summary>Checks if a complete mailflag matches the format and list of mod flags.</summary>
        public static bool IsFullFlag(string fullFlagName)
        {
            return fullFlagName.StartsWith(FLAG_BASE) && FlagList.Contains(fullFlagName.Skip(FLAG_BASE.Length));
        }

        /// <summary>Checks if a mod flag exists in a player's mailReceived</summary>
        /// <param name="flagName">Name of the flag to check.</param>
        /// <param name="who">Player to check in mailReceived.</param>
        /// <returns></returns>
        public static bool HasFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return false;
            }
            return who.mailReceived.Contains(FLAG_BASE + flagName);
        }

        /// <summary>Adds a mod flag to a player's mailReceived</summary>
        /// <param name="flagName">Name of the flag to add.</param>
        /// <param name="who">Player to add flag to.</param>
        public static void AddFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return;
            }
            if (!who.mailReceived.Contains(FLAG_BASE + flagName))
            {
                who.mailReceived.Add(FLAG_BASE + flagName);
            }
        }

        /// <summary>Adds multiple mod flags to a player's mailReceived</summary>
        /// <param name="flagNames">List of the flag names to add.</param>
        /// <param name="who">Player to add flags to.</param>
        public static void AddFlags(List<string> flagNames, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            foreach (string flag in flagNames) { AddFlag(flag, who); }
        }

        /// <summary>Removes a mod flag from a player's mailReceived, if present</summary>
        /// <param name="flagName">Name of the flag to remove.</param>
        /// <param name="who">Player to remove flag from.</param>
        public static void RemoveFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return;
            }
            while (who.mailReceived.Contains(FLAG_BASE + flagName))
            {
                who.mailReceived.Remove(FLAG_BASE + flagName);
            }
        }

        /// <summary>Removes multiple mod flags from a player's mailReceived</summary>
        /// <param name="flagNames">List of the flag names to remove.</param>
        /// <param name="who">Player to remove flags from.</param>
        public static void RemoveFlags(List<string> flagNames, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            foreach (string flag in flagNames) { RemoveFlag(flag, who); }
        }
    }
}