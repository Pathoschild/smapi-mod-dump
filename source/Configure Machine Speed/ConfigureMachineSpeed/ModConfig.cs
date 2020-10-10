/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BayesianBandit/ConfigureMachineSpeed
**
*************************************************/

using System.Globalization;
using System.Linq;
using StardewModdingAPI;

namespace ConfigureMachineSpeed
{
    internal class ModConfig
    {
        public uint UpdateInterval { get; set; } = 10;
        public MachineConfig[] Machines { get; set; }
        public SButton? ReloadConfigKey { get; set; } = SButton.L;

        public ModConfig()
        {
            Machines = DefaultMachines();
        }

        private MachineConfig[] DefaultMachines()
        {
            string[] names = { "Bee House", "Cask", "Charcoal Kiln", "Cheese Press", "Crystalarium",
                "Furnace", "Incubator", "Keg", "Lightning Rod", "Loom", "Mayonnaise Machine", "Oil Maker",
                "Preserves Jar", "Recycling Machine", "Seed Maker", "Slime Egg-Press", "Slime Incubator",
                "Tapper", "Worm Bin" };
            return names.Select(x => new MachineConfig(x)).ToArray();
        }
    }

    internal class MachineConfig
    {
        public string Name { get; set; }
        public float Time { get; set; } = 100f;
        public bool UsePercent { get; set; } = true;

        public MachineConfig(string Name)
        {
            this.Name = Name;
        }
    }

}
