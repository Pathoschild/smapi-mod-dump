/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Sit-n-Relax
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Newtonsoft.Json;

namespace SitnRelax
{
    class configModel
    {
        public int regenRate { get; set; } = 6; //how many seconds per regen tick 
        public int stamRegen { get; set; } = 2; //amount of stamina regenerated per tick
        public int healthRegen { get; set; } = 2; //amount of health regenerated per tick
        public string restMessage { get; set; } = "sits down and relaxes!"; //Added as a hudMessage.
    }
}
