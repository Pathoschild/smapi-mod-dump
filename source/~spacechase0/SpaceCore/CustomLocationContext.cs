/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace SpaceCore
{
    internal class CustomLocationContext
    {
        public string Name { get; set; }
        public Func< Random, LocationWeather > GetLocationWeatherForTomorrow { get; set; }
        //public Func< Farmer, string > PassoutWakeupLocation { get; set; }
        //public Func< Farmer, Point? > PassoutWakeupPoint { get; set; }
    }
}
