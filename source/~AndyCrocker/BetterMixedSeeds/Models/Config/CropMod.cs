/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System;

namespace BetterMixedSeeds.Models.Config
{
    /// <summary>Data about a mod.</summary>
    public class CropMod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether the crops in the mod can be planted from mixed seeds.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>The season object that contains spring crops.</summary>
        public Season Spring { get; set; }

        /// <summary>The season object that contains summer crops.</summary>
        public Season Summer { get; set; }

        /// <summary>The season object that contains fall crops.</summary>
        public Season Fall { get; set; }

        /// <summary>The season object that contains winter crops.</summary>
        public Season Winter { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="spring">The season object that contains spring crops.</param>
        /// <param name="summer">The season object that contains summer crops.</param>
        /// <param name="fall">The season object that contains fall crops.</param>
        /// <param name="winter">The season object that contains winter crops.</param>
        /// <param name="enabled">Whether the crops in the mod can be from in mixed seeds.</param>
        public CropMod(Season spring, Season summer, Season fall, Season winter, bool enabled)
        {
            Spring = spring;
            Summer = summer;
            Fall = fall;
            Winter = winter;
            Enabled = enabled;
        }

        /// <summary>Gets a season property by name.</summary>
        /// <param name="seasonName">The name of the season property to get.</param>
        /// <returns>The <see cref="Season"/> property with the specified name.</returns>
        public Season GetSeasonByName(string seasonName)
        {
            switch (seasonName.ToLower())
            {
                case "spring": return Spring;
                case "summer": return Summer;
                case "fall": return Fall;
                case "winter": return Winter;
                default: return null;
            }
        }
    }
}
