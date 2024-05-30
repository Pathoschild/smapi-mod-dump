/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterPigs
{
    public class Config
    {
        public List<string> AnimalsGoOutsideHorribleWeather { get; set; } = [];

        public List<string> AnimalsGoOutsideBadWeather { get; set; } = ["Pig"];

        public List<string> AnimalsStayInside { get; set; } = [];

        public bool DelayedPet { get; set; } = true;

        public double PigAnimalCrackerMultiplier { get; set; } = 2.0;

        public bool NoTruffleLimit { get; set; } = true;
    }
}
