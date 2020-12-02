/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishInfo
{
    class ModConfig
    {
        /// <summary>
        /// To require fish being caught before showing the information
        /// </summary>
        public bool RequireFishCaughtForFullInfo { get; set; } = true;
        
        /// <summary>
        /// Shows the name of uncaught fish
        /// Does nothing if RequireFishCaught is false
        /// </summary>
        public bool UncaughtFishAlwaysShowName { get; set; } = true;

        /// <summary>
        /// Shows the location of uncaught fish
        /// Does nothing if RequireFishCaught is false
        /// </summary>
        public bool UncaughtFishAlwaysShowLocation { get; set; } = false;

        /// <summary>
        /// Shows the season uncaught fish are available in
        /// Does nothing if RequireFishCaught is false
        /// </summary>
        public bool UncaughtFishAlwaysShowSeason { get; set; } = false;

        /// <summary>
        /// Shows the time uncaught fish are available at
        /// Does nothing if RequireFishCaught is false
        /// </summary>
        public bool UncaughtFishAlwaysShowTime { get; set; } = false;

        /// <summary>
        /// Shows the weather uncaught fish are available in
        /// Does nothing if RequireFishCaught is false
        /// </summary>
        public bool UncaughtFishAlwaysShowWeather { get; set; } = false;
    }
}
