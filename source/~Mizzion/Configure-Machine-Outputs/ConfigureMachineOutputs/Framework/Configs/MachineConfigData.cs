/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class MachineConfigData
    {
        public bool Enabled { get; set; } = true;

        public string MachineName { get; set; }

        private static string Id;

        internal string QualityId = Id;

        public List<MachineOutputRule> OutPut { get; set; } = new();


        public static string SetId(string id)
        {
            return id;
        }        

        /*
        public static MachineData GetMachineData()
        {
            var machines = DataLoader.Machines(Game1.content);
            
        }*/
    }
}
