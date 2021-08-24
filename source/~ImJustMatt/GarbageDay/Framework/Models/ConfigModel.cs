/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace GarbageDay.Framework.Models
{
    internal class ConfigModel
    {
        /// <summary>Global chance that a random item from season is collected</summary>
        public double GetRandomItemFromSeason { get; set; } = 0.1;

        /// <summary>Day of week that trash is emptied out</summary>
        public int GarbageDay { get; set; } = 1;

        /// <summary>Adds IsIgnored to all Garbage Cans every day</summary>
        public bool HideFromChestsAnywhere { get; set; } = true;

        /// <summary>Edit all Maps instead of specific maps</summary>
        public bool Debug { get; set; } = false;

        /// <summary>Log Level used when loading in garbage cans</summary>
        public string LogLevel { get; set; } = "Trace";

        /// <summary>Loot available to all garbage cans</summary>
        public Dictionary<string, double> GlobalLoot { get; set; } = new()
        {
            {"item_trash", 1},
            {"item_joja_cola", 1},
            {"item_broken_glasses", 1},
            {"item_broken_cd", 1},
            {"item_soggy_newspaper", 1},
            {"item_bread", 1},
            {"item_field_snack", 1},
            {"item_acorn", 0.3},
            {"item_maple_seed", 0.3},
            {"item_pine_cone", 0.3},
            {"item_green_algae", 0.3}
        };
    }
}