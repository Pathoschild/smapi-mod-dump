/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    /// <summary>The new configuration model.</summary>
    public class NewModConfig
    {
        public bool EnableTrash { get; set; } = true;
        public bool RequiresBait { get; set; } = true;
        public int PercentChanceForTrash { get; set; } = 20;
        public bool EnableBetterQuality { get; set; } = false;
        public bool EnablePassiveTrash { get; set; } = false;
        public int PercentChanceForPassiveTrash { get; set; } = 20;
        public Dictionary<int, int> WhatCanBeFoundAsPassiveTrash { get; set; }
        public ConfigItem AllWater { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem FarmLand { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem CindersapForest { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem MountainsLake { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Town { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Mines_Layer20 { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Mines_Layer60 { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Mines_Layer100 { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem MutantBugLair { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem WitchsSwamp { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem SecretWoods { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Desert { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Sewers { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
        public ConfigItem Beach { get; set; } = new ConfigItem(new List<Item>(), new List<Item>());
    }
}
