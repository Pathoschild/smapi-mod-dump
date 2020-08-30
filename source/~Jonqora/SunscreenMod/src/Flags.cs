using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SunscreenMod
{
    class Flags
    {
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;

        public static string FlagBase { get; } = "6676SunscreenMod.";
        static string[] FlagList { get; } = {
            "NewDay",
            "HasAppliedAloeToday",
            "SunburnLevel_1",
            "SunburnLevel_2",
            "SunburnLevel_3",
            "NewBurnDamageLevel_1",
            "NewBurnDamageLevel_2",
            "NewBurnDamageLevel_3",
            "NormalSkinColor_1" //And all the others
        };

        public static bool IsFlag(string flagName)
        {
            return FlagList.Contains(flagName);
        }
        public static bool IsFullFlag(string fullFlagName)
        {
            return fullFlagName.StartsWith(FlagBase) && FlagList.Contains(fullFlagName.Skip(FlagBase.Length));
        }

        public static bool HasFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return false;
            }
            return who.mailReceived.Contains(FlagBase + flagName);
        }

        public static void AddFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return;
            }
            if (!who.mailReceived.Contains(FlagBase + flagName))
            {
                who.mailReceived.Add(FlagBase + flagName);
            }
        }

        public static void AddFlags(List<string> flagNames, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            foreach (string flag in flagNames) { AddFlag(flag, who); }
        }

        public static void RemoveFlag(string flagName, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            if (!IsFlag(flagName))
            {
                Monitor.Log($"ERROR: flag {flagName} does not exist in the Flags list.", LogLevel.Warn);
                return;
            }
            while (who.mailReceived.Contains(FlagBase + flagName))
            {
                who.mailReceived.Remove(FlagBase + flagName);
            }
        }

        public static void RemoveFlags(List<string> flagNames, Farmer who = null)
        {
            who = who ?? Game1.player; //Default to the current player
            foreach (string flag in flagNames) { RemoveFlag(flag, who); }
        }
    }
}
