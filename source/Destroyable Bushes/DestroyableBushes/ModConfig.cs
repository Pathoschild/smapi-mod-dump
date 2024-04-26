/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace DestroyableBushes
{
    /// <summary>A collection of this mod's config.json file settings.</summary>
    public class ModConfig
    {
        /// <summary>The number of axe upgrades required to destroy non-tea bushes. 0 allows the default axe to remove bushes, 1 requires the copper axe or better, etc.</summary>
        public int AxeUpgradesRequired { get; set; } = 0;

        /// <summary>The number component of <see cref="WhenBushesRegrow"/>.</summary>
        [JsonIgnore]
        public int? regrowNumber = 3;
        /// <summary>The unit component of <see cref="WhenBushesRegrow"/>.</summary>
        [JsonIgnore]
        public RegrowUnit? regrowUnit = RegrowUnit.Days;

        private string whenBushesRegrow = "3 days";
        /// <summary>A string describing the amount of time that will pass before a destroyed bush respawns. Set to null if unrecognized; null disables respawning.</summary>
        public string WhenBushesRegrow
        {
            get
            {
                return whenBushesRegrow;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("null", StringComparison.OrdinalIgnoreCase)) //if the provided value is null, blank, or "null"
                {
                    regrowNumber = null; //null the number
                    regrowUnit = null; //null the unit
                    whenBushesRegrow = null; //null this field
                    return; //skip the rest of this process
                }

                if (value.IndexOf("day", StringComparison.OrdinalIgnoreCase) >= 0) //if this value contains "day"
                {
                    regrowUnit = RegrowUnit.Days; //set the unit
                }
                else if (value.IndexOf("season", StringComparison.OrdinalIgnoreCase) >= 0 || value.IndexOf("month", StringComparison.OrdinalIgnoreCase) >= 0) //if this value contains "season" or "month"
                {
                    regrowUnit = RegrowUnit.Seasons; //set the unit
                }
                else if (value.IndexOf("year", StringComparison.OrdinalIgnoreCase) >= 0) //if this value contains "year"
                {
                    regrowUnit = RegrowUnit.Years; //set the unit
                }
                else //if no unit was identified
                {
                    ModEntry.Instance.Monitor.Log($"WhenBushesRegrow setting disabled: Unit of time not found. Please include \"days\", \"seasons\", or \"years\".", LogLevel.Warn);
                    regrowNumber = null; //null the number
                    regrowUnit = null; //null the unit
                    whenBushesRegrow = null; //null this field
                    return; //skip the rest of this process
                }

                string trimmedValue = value.Trim(); //the value with "outer" whitespace removed

                string numberString = null; //the value's number component

                for (int x = 0; x < trimmedValue.Length; x++) //for each character in the value
                {
                    if (char.IsDigit(trimmedValue[x]) == false) //if this character is NOT a digit (0-9)
                    {
                        numberString = trimmedValue.Substring(0, x); //get all the characters before this one (if any)
                        break; //skip the rest of this loop
                    }
                }

                if (numberString?.Length > 0 && int.TryParse(numberString, out int result)) //if the number string can be parsed into an integer
                {
                    regrowNumber = result; //set the number
                    whenBushesRegrow = value; //set this field to the unmodified value
                }
                else
                {
                    ModEntry.Instance.Monitor.Log($"WhenBushesRegrow setting disabled: Number not found. Please start it with a valid integer, e.g. \"3 days\" or \"1 season\".", LogLevel.Warn);
                    regrowNumber = null; //null the number
                    regrowUnit = null; //null the unit
                    whenBushesRegrow = null; //null this field
                }
            }
        }

        /// <summary>A list of in-game locations where bushes should be made destroyable. If blank, all locations will be allowed.</summary>
        public List<string> DestroyableBushLocations { get; set; } = new List<string>();
        /// <summary>A list of bush types that are allowed to be destroyed.</summary>
        public DestroyableBushTypes DestroyableBushTypes { get; set; } = new DestroyableBushTypes();

        /// <summary>The number of wood pieces dropped by each type of bush when destroyed.</summary>
        public AmountOfWoodDropped AmountOfWoodDropped { get; set; } = new AmountOfWoodDropped();
    }

    /// <summary>A group of config.json file settings. Determines which bush sizes are allowed to be destroyed.</summary>
    public class DestroyableBushTypes
    {
        public bool SmallBushes { get; set; } = true;
        public bool MediumBushes { get; set; } = true;
        public bool LargeBushes { get; set; } = true;
        public bool WalnutBushes { get; set; } = true;
    }

    /// <summary>A group of config.json file settings. Sets the number of wood pieces dropped by each type of bush when destroyed.</summary>
    public class AmountOfWoodDropped
    {
        public int SmallBushes { get; set; } = 2;
        public int MediumBushes { get; set; } = 4;
        public int LargeBushes { get; set; } = 8;
        public int WalnutBushes { get; set; } = 4;
        public int GreenTeaBushes { get; set; } = 0;
    }

    /// <summary>The units of time used by the <see cref="ModConfig.WhenBushesRegrow"/> setting.</summary>
    public enum RegrowUnit
    {
        Days,
        Seasons,
        Years
    }
}
