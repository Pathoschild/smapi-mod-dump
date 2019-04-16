using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    class NewModConfig
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
